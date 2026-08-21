import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';

import '../config/api_config.dart';
import 'api_exception.dart';
import 'api_response.dart';

typedef AccessTokenProvider = Future<String?> Function();
typedef UnauthorizedCallback = Future<String?> Function();

class ApiClient {
  factory ApiClient({
    required ApiConfig config,
    required http.Client httpClient,
    AccessTokenProvider? accessTokenProvider,
    UnauthorizedCallback? onUnauthorized,
    Duration timeout = const Duration(seconds: 15),
  }) => ApiClient._(
    config.baseUrl,
    httpClient,
    accessTokenProvider,
    onUnauthorized,
    timeout,
  );

  ApiClient._(
    this._baseUrl,
    this._httpClient,
    this._accessTokenProvider,
    this._onUnauthorized,
    this._timeout,
  );

  final Uri _baseUrl;
  final http.Client _httpClient;
  final AccessTokenProvider? _accessTokenProvider;
  final UnauthorizedCallback? _onUnauthorized;
  final Duration _timeout;

  Future<ApiResponse> get(String path) => _send('GET', path);

  Future<ApiResponse> post(String path, {Object? body}) =>
      _send('POST', path, body: body);

  Future<ApiResponse> put(String path, {Object? body}) =>
      _send('PUT', path, body: body);

  Future<ApiResponse> patch(String path, {Object? body}) =>
      _send('PATCH', path, body: body);

  Future<ApiResponse> delete(String path, {Object? body}) =>
      _send('DELETE', path, body: body);

  /// [timeout], varsayılan istemci bütçesini yalnız bu istek için değiştirir.
  ///
  /// Varsayılan 15 saniye normal JSON çağrıları için doğru, ama dış bir servise
  /// giden yüklemeler için değil: fiş okuma sunucuda 60 saniyeye kadar
  /// bekleyebiliyor ve 18 Ağustos 2026 ölçümünde tek bir okuma 46,9 saniye
  /// sürdü. 15 saniyelik bütçe, sunucu fişi okumuşken isteği kesip kullanıcıya
  /// "zaman aşımı" derdi.
  Future<ApiResponse> postMultipart(
    String path, {
    required ApiUpload upload,
    Map<String, String> fields = const {},
    Duration? timeout,
  }) => _sendMultipart(path, upload, fields, timeout: timeout);

  Future<ApiBinaryResponse> getBytes(String path) => _sendBytes(path);

  Future<ApiResponse> _sendMultipart(
    String path,
    ApiUpload upload,
    Map<String, String> fields, {
    bool allowUnauthorizedRetry = true,
    String? accessTokenOverride,
    Duration? timeout,
  }) async {
    final request = http.MultipartRequest('POST', _resolve(path));
    request.headers['Accept'] = 'application/json';
    final token = accessTokenOverride ?? await _accessTokenProvider?.call();
    if (token != null && token.trim().isNotEmpty) {
      request.headers['Authorization'] = 'Bearer ${token.trim()}';
    }
    request.fields.addAll(fields);
    request.files.add(
      http.MultipartFile.fromBytes(
        upload.fieldName,
        upload.bytes,
        filename: upload.fileName,
        contentType: upload.mediaType == null
            ? null
            : MediaType.parse(upload.mediaType!),
      ),
    );
    final response = await _sendRequest(request, timeout: timeout);
    final decoded = _decodeBytes(response.bodyBytes);
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return ApiResponse(statusCode: response.statusCode, data: decoded);
    }
    if (response.statusCode == 401 &&
        allowUnauthorizedRetry &&
        _onUnauthorized != null) {
      final refreshedToken = await _onUnauthorized.call();
      if (refreshedToken != null && refreshedToken.trim().isNotEmpty) {
        return _sendMultipart(
          path,
          upload,
          fields,
          allowUnauthorizedRetry: false,
          accessTokenOverride: refreshedToken,
          timeout: timeout,
        );
      }
    }
    throw _failure(response.statusCode, decoded);
  }

  Future<ApiBinaryResponse> _sendBytes(
    String path, {
    bool allowUnauthorizedRetry = true,
    String? accessTokenOverride,
  }) async {
    final request = http.Request('GET', _resolve(path));
    final token = accessTokenOverride ?? await _accessTokenProvider?.call();
    if (token != null && token.trim().isNotEmpty) {
      request.headers['Authorization'] = 'Bearer ${token.trim()}';
    }
    final streamed = await _sendStreamed(request);
    final bytes = await streamed.stream.toBytes();
    if (streamed.statusCode >= 200 && streamed.statusCode < 300) {
      return ApiBinaryResponse(
        bytes: bytes,
        contentType: streamed.headers['content-type'],
        contentDisposition: streamed.headers['content-disposition'],
      );
    }
    if (streamed.statusCode == 401 &&
        allowUnauthorizedRetry &&
        _onUnauthorized != null) {
      final refreshedToken = await _onUnauthorized.call();
      if (refreshedToken != null && refreshedToken.trim().isNotEmpty) {
        return _sendBytes(
          path,
          allowUnauthorizedRetry: false,
          accessTokenOverride: refreshedToken,
        );
      }
    }
    final decoded = _decode(utf8.decode(bytes, allowMalformed: true));
    throw _failure(streamed.statusCode, decoded);
  }

  Future<ApiResponse> _send(
    String method,
    String path, {
    Object? body,
    bool allowUnauthorizedRetry = true,
    String? accessTokenOverride,
  }) async {
    final request = http.Request(method, _resolve(path));
    request.headers['Accept'] = 'application/json';

    final token = accessTokenOverride ?? await _accessTokenProvider?.call();
    if (token != null && token.trim().isNotEmpty) {
      request.headers['Authorization'] = 'Bearer ${token.trim()}';
    }

    if (body != null) {
      request.headers['Content-Type'] = 'application/json; charset=utf-8';
      request.body = jsonEncode(body);
    }

    final response = await _sendRequest(request);

    final decoded = _decodeBytes(response.bodyBytes);
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return ApiResponse(statusCode: response.statusCode, data: decoded);
    }

    if (response.statusCode == 401 &&
        allowUnauthorizedRetry &&
        _onUnauthorized != null) {
      final refreshedToken = await _onUnauthorized.call();
      if (refreshedToken != null && refreshedToken.trim().isNotEmpty) {
        return _send(
          method,
          path,
          body: body,
          allowUnauthorizedRetry: false,
          accessTokenOverride: refreshedToken,
        );
      }
    }

    throw _failure(response.statusCode, decoded);
  }

  Future<http.Response> _sendRequest(
    http.BaseRequest request, {
    Duration? timeout,
  }) async =>
      http.Response.fromStream(await _sendStreamed(request, timeout: timeout));

  Future<http.StreamedResponse> _sendStreamed(
    http.BaseRequest request, {
    Duration? timeout,
  }) async {
    try {
      return await _httpClient.send(request).timeout(timeout ?? _timeout);
    } on TimeoutException {
      throw ApiException.timeout();
    } on http.ClientException {
      throw ApiException.network();
    }
  }

  ApiException _failure(int statusCode, Object? decoded) =>
      decoded is Map<String, dynamic>
      ? ApiException.fromProblemDetails(statusCode, decoded)
      : ApiException(
          statusCode: statusCode,
          code: 'server.request_failed',
          message: 'İstek tamamlanamadı.',
        );

  Uri _resolve(String path) {
    final relativePath = path.startsWith('/') ? path.substring(1) : path;
    return _baseUrl.resolve(relativePath);
  }

  static Object? _decode(String body) {
    if (body.trim().isEmpty) {
      return null;
    }

    try {
      return jsonDecode(body);
    } on FormatException {
      return null;
    }
  }

  static Object? _decodeBytes(List<int> bytes) =>
      _decode(utf8.decode(bytes, allowMalformed: true));
}

class ApiUpload {
  const ApiUpload({
    required this.bytes,
    required this.fileName,
    this.fieldName = 'file',
    this.mediaType,
  });

  final List<int> bytes;
  final String fileName;
  final String fieldName;
  final String? mediaType;
}

class ApiBinaryResponse {
  const ApiBinaryResponse({
    required this.bytes,
    this.contentType,
    this.contentDisposition,
  });

  final List<int> bytes;
  final String? contentType;
  final String? contentDisposition;
}
