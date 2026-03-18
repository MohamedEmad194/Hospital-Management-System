# 🔧 إعداد تسجيل الدخول للاختبار - دليل شامل

## المشكلة الحالية:
- البريد الإلكتروني غير موجود في النظام
- رسائل الخطأ تحتوي على معلومات حساسة (تم إصلاحها في الكود)

## الحل السريع:

### 1. إنشاء مستخدم تجريبي للاختبار:

**استخدم Postman أو المتصفح:**

```
POST http://localhost:5230/api/TestCredentials/create-test-user
Content-Type: application/json

{
  "email": "test@hospital.com",
  "password": "Test@1234",
  "role": "Patient",
  "firstName": "Test",
  "lastName": "User"
}
```

**أو استخدم curl:**
```bash
curl -X POST http://localhost:5230/api/TestCredentials/create-test-user \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"test@hospital.com\",\"password\":\"Test@1234\",\"role\":\"Patient\",\"firstName\":\"Test\",\"lastName\":\"User\"}"
```

### 2. إنشاء حسابات لجميع الكيانات الموجودة:

```
POST http://localhost:5230/api/TestCredentials/ensure-users
```

هذا سينشئ حسابات User لجميع:
- الأطباء (Doctors)
- المرضى (Patients)  
- الموظفين (Staff)

### 3. الحصول على بيانات تسجيل الدخول:

```
GET http://localhost:5230/api/TestCredentials/all
```

## بيانات تسجيل الدخول الافتراضية:

بعد تشغيل `ensure-users`، يمكنك استخدام:

**Admin:**
- Email: `admin@hospital.com`
- Password: `Admin@123`
- Role: `Admin`

**Doctor:**
- Email: أي بريد طبيب موجود في قاعدة البيانات
- Password: `Doctor@123`
- Role: `Doctor`

**Patient:**
- Email: أي بريد مريض موجود في قاعدة البيانات
- Password: `Patient@123`
- Role: `Patient`

**Staff/Nurse:**
- Email: أي بريد موظف موجود في قاعدة البيانات
- Password: `Staff@123`
- Role: `Nurse` أو `Staff`

## ملاحظات مهمة:

1. **أوقف التطبيق أولاً** قبل إعادة البناء:
   - أوقف التطبيق الذي يعمل (process 22816)
   - ثم أعد البناء

2. **رسائل الخطأ الآن آمنة** - لا تحتوي على معلومات حساسة

3. **النظام ينشئ حسابات تلقائياً** - إذا كان البريد موجوداً في Doctors/Patients/Staff ولكن ليس لديه حساب User، سيتم إنشاؤه تلقائياً عند محاولة تسجيل الدخول

## خطوات التشخيص:

1. تأكد من أن Backend يعمل على `http://localhost:5230`
2. أنشئ مستخدم تجريبي باستخدام `/api/TestCredentials/create-test-user`
3. حاول تسجيل الدخول بالبيانات التي أنشأتها
4. إذا فشل، تحقق من Console في المتصفح لرؤية تفاصيل الخطأ

