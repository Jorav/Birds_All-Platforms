# 📱 Birds.Android

This is the Android-specific project of the **Birds** application. It depends on shared logic from the root `Birds` folder and is configured to automatically generate a **signed Android App Bundle (AAB)** when the application is built or run.

> ⚠️ Before running or publishing the app, you must set up signature credentials.

---

## 🧩 Setup Steps (Required Before Running)

### 1. Create a `Secrets.props` File

Create a `Secrets.props` file in the `Birds.Android` folder. This file contains your signing credentials:

```xml
<Project>
  <PropertyGroup>
    <AndroidSigningKeyStore>keystore_name.keystore</AndroidSigningKeyStore>
    <AndroidSigningKeyAlias>key_alias</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass>key_password</AndroidSigningKeyPass>
    <AndroidSigningStorePass>store_password</AndroidSigningStorePass>
  </PropertyGroup>
</Project>
```

> 💡 Do **not** commit this file to version control. Add it to `.gitignore`.

---

### 2. Create a Keystore

Use the [`keytool`](https://www.oracle.com/java/technologies/javase-downloads.html) command to generate a new keystore in the Birds.Android project folder that meets Google Play requirements:

```bash
keytool -genkeypair -v \
  -keystore keystore_name.keystore \
  -alias key_alias \
  -keyalg RSA \
  -keysize 2048 \
  -validity 36500
```

Save the file securely — losing it means you cannot update your app on Google Play.

---

## ▶️ Run the App

Once the above steps are complete, you can run or publish the app and it will automatically generate a **signed AAB**.

For more details, see the official [Android App Signing](https://developer.android.com/studio/publish/app-signing) documentation.
