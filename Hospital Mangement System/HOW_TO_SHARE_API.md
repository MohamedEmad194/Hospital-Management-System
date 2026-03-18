# كيفية تسليم API لمطور Flutter

## 📦 الملفات المطلوب إرسالها

### 1. ملف التوثيق الشامل
```
FLUTTER_API_GUIDE.md
```
هذا الملف يحتوي على:
- جميع نقاط النهاية (Endpoints)
- أمثلة على الطلبات والردود
- أمثلة كود Flutter
- شرح طريقة المصادقة

### 2. ملف Postman Collection
```
Postman/HMS_Flutter.postman_collection.json
```
يمكن للمطور استيراده في Postman لاختبار الـ API مباشرة

### 3. ملف التوثيق الكامل (اختياري)
```
API_DOCUMENTATION.md
```
يحتوي على تفاصيل أكثر عن الـ API

---

## 📧 كيفية الإرسال

### الطريقة 1: إرسال الملفات مباشرة
1. أرسل الملفات التالية:
   - `FLUTTER_API_GUIDE.md`
   - `Postman/HMS_Flutter.postman_collection.json`

### الطريقة 2: مشاركة من خلال Git
```
أرسل له الرابط:
- FLUTTER_API_GUIDE.md
- Postman/HMS_Flutter.postman_collection.json
```

### الطريقة 3: مشاركة Postman Collection عبر Link
1. افتح Postman
2. استورد `HMS_Flutter.postman_collection.json`
3. اضغط على Share
4. أنشئ Public Link
5. أرسل الرابط للمطور

---

## 🔑 معلومات مهمة يجب إرسالها

### 1. Base URL
```
Development: http://localhost:5230
أو
Development: https://localhost:7102
```

### 2. بيانات تسجيل الدخول للاختبار (إن وجدت)
```
Email: admin@hospital.com
Password: Admin@123
```

### 3. معلومات CORS (إذا كانت مطلوبة)
إذا كان التطبيق Flutter يعمل على web، قد تحتاج لتكوين CORS في الـ API

---

## 📋 خطوات المطور بعد استلام الملفات

### 1. قراءة التوثيق
- افتح `FLUTTER_API_GUIDE.md`
- راجع جميع الـ Endpoints
- فهم طريقة المصادقة

### 2. اختبار الـ API باستخدام Postman
- استورد `HMS_Flutter.postman_collection.json`
- جرب Login للحصول على Token
- جرب بعض الـ Endpoints

### 3. تطبيق الكود في Flutter
- استخدم الأمثلة الموجودة في `FLUTTER_API_GUIDE.md`
- أنشئ ApiService class
- نفذ جميع الوظائف المطلوبة

---

## 🚀 Quick Start للمطور

### الخطوة 1: تسجيل الدخول
```dart
POST /api/auth/login
{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

### الخطوة 2: حفظ Token
```dart
String token = response['token'];
// احفظه في SharedPreferences
```

### الخطوة 3: استخدام Token في جميع الطلبات
```dart
headers: {
  'Authorization': 'Bearer $token',
  'Content-Type': 'application/json',
}
```

---

## ⚠️ ملاحظات مهمة

1. **HTTPS في Production:** تأكد من استخدام HTTPS عند النشر
2. **Security:** لا تحفظ الـ Token في كود التطبيق، استخدم Secure Storage
3. **Error Handling:** تأكد من معالجة جميع الأخطاء
4. **Token Expiration:** تحقق من صلاحية الـ Token قبل كل طلب

---

## 📞 في حالة وجود أسئلة

إذا كان لدى المطور أي أسئلة، يمكنه:
1. مراجعة `FLUTTER_API_GUIDE.md`
2. اختبار الـ API في Postman
3. مراجعة `API_DOCUMENTATION.md` للتفاصيل الكاملة

---

**ملاحظة:** تأكد من أن الـ Backend يعمل قبل البدء في التطوير!
