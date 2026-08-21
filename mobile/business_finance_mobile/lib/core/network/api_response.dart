class ApiResponse {
  const ApiResponse({required this.statusCode, required this.data});

  final int statusCode;
  final Object? data;

  Map<String, dynamic> requireObject() {
    final value = data;
    if (value is Map<String, dynamic>) {
      return value;
    }

    throw const FormatException('API response must be a JSON object.');
  }
}
