pluginManagement {
    val flutterSdkPath =
        run {
            val properties = java.util.Properties()
            file("local.properties").inputStream().use { properties.load(it) }
            val flutterSdkPath = properties.getProperty("flutter.sdk")
            require(flutterSdkPath != null) { "flutter.sdk not set in local.properties" }
            flutterSdkPath
        }

    includeBuild("$flutterSdkPath/packages/flutter_tools/gradle")

    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}

plugins {
    id("dev.flutter.flutter-plugin-loader") version "1.0.0"
    // AGP 9'a bilerek çıkılmıyor (Flutter 3.44 şablonunun varsayılanı 9.0.1).
    // AGP 9 kendi Kotlin desteğini getiriyor ve kendi Kotlin Gradle eklentisini
    // uygulayan bir bağımlılık derlenmiyor; bugün `share_plus` böyle ve
    // 13.3.0'da da aynı. Kırılma yalnız temiz derlemede görünüyordu, çünkü
    // Gradle önbelleği eski çıktıyı tutuyordu. Eklenti ekosistemi yetiştiğinde
    // 9'a çıkılır.
    id("com.android.application") version "8.13.0" apply false
    id("org.jetbrains.kotlin.android") version "2.3.20" apply false
}

include(":app")
