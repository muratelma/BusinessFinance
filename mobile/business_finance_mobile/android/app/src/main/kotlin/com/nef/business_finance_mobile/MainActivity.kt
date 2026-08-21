package com.nef.business_finance_mobile

import android.app.Activity
import android.content.Intent
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel

class MainActivity : FlutterActivity() {
    private val channelName = "com.nef.business_finance_mobile/document_file"
    private val createDocumentRequest = 4107
    private var pendingBytes: ByteArray? = null
    private var pendingResult: MethodChannel.Result? = null

    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)
        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, channelName)
            .setMethodCallHandler { call, result ->
                if (call.method != "save") {
                    result.notImplemented()
                    return@setMethodCallHandler
                }
                if (pendingResult != null) {
                    result.error("save_in_progress", "Başka bir dosya kaydetme işlemi sürüyor.", null)
                    return@setMethodCallHandler
                }
                val bytes = call.argument<ByteArray>("bytes")
                val fileName = call.argument<String>("fileName")
                val mimeType = call.argument<String>("mimeType")
                if (bytes == null || fileName.isNullOrBlank() || mimeType.isNullOrBlank()) {
                    result.error("invalid_file", "Kaydedilecek dosya bilgileri geçersiz.", null)
                    return@setMethodCallHandler
                }
                pendingBytes = bytes
                pendingResult = result
                val intent = Intent(Intent.ACTION_CREATE_DOCUMENT).apply {
                    addCategory(Intent.CATEGORY_OPENABLE)
                    type = mimeType
                    putExtra(Intent.EXTRA_TITLE, fileName)
                }
                startActivityForResult(intent, createDocumentRequest)
            }
    }

    @Deprecated("Deprecated in Android; retained for FlutterActivity result forwarding.")
    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode != createDocumentRequest) return
        val result = pendingResult
        val bytes = pendingBytes
        pendingResult = null
        pendingBytes = null
        if (resultCode != Activity.RESULT_OK || data?.data == null) {
            result?.success(false)
            return
        }
        if (bytes == null) {
            result?.error("save_failed", "Dosya cihaza kaydedilemedi.", null)
            return
        }
        try {
            contentResolver.openOutputStream(data.data!!, "w").use { output ->
                checkNotNull(output) { "Dosya yazma akışı açılamadı." }
                output.write(bytes)
            }
            result?.success(true)
        } catch (error: Exception) {
            result?.error("save_failed", "Dosya cihaza kaydedilemedi.", null)
        }
    }
}
