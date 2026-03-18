# تشخيص أخطاء 500 في Appointments و Bills

## المشكلة:
خطأ 500 عند محاولة الوصول إلى `/api/Appointments` أو `/api/Bills`

## الأسباب المحتملة:

### 1. مشكلة في المصادقة (Authorization)
- الـ endpoints تتطلب `[Authorize]`
- يجب التأكد من أن Token موجود وصحيح
- تحقق من Console في المتصفح لرؤية رسالة الخطأ

### 2. مشكلة في Navigation Properties
- قد تكون Patient أو Doctor null في قاعدة البيانات
- قد تكون Foreign Keys مفقودة

### 3. مشكلة في AutoMapper
- قد يفشل عند محاولة الوصول إلى FirstName/LastName من null objects

## الحل:

### الخطوة 1: تحقق من Token
افتح Console في المتصفح وتحقق من:
- هل Token موجود في localStorage?
- هل Token صحيح؟

### الخطوة 2: تحقق من رسالة الخطأ
افتح Network tab وتحقق من Response للخطأ 500:
```
GET http://localhost:5230/api/Appointments
```

يجب أن ترى رسالة خطأ مفصلة الآن.

### الخطوة 3: تحقق من Logs في Backend
افتح Backend console وتحقق من:
- رسائل Log التي تظهر تفاصيل الخطأ
- Stack trace كامل

### الخطوة 4: التحقق من البيانات
تحقق من أن:
- Patient records موجودة ومربوطة بشكل صحيح
- Doctor records موجودة ومربوطة بشكل صحيح
- Foreign keys صحيحة

## التحسينات المضافة:

1. ✅ معالجة أخطاء محسّنة مع تفاصيل كاملة
2. ✅ حماية من null في AutoMapper
3. ✅ Logging محسّن في Controllers
4. ✅ رسائل خطأ أوضح في Response

## اختبار سريع:

```bash
# تحقق من Appointments (يتطلب تسجيل دخول)
GET http://localhost:5230/api/Appointments
Authorization: Bearer YOUR_TOKEN

# تحقق من Bills (يتطلب تسجيل دخول)
GET http://localhost:5230/api/Bills
Authorization: Bearer YOUR_TOKEN
```

