# إنشاء Migration لجدول NursingUnits

## المشكلة:
جدول `NursingUnits` غير موجود في قاعدة البيانات، مما يسبب خطأ 500 عند محاولة جلب البيانات.

## الحل:

### 1. إيقاف الخادم (Backend)
أوقف تشغيل الخادم أولاً.

### 2. إنشاء Migration

افتح Terminal في مجلد المشروع:
```bash
cd "Hospital Mangement System\Hospital Mangement System"

# إنشاء migration جديد
dotnet ef migrations add AddNursingUnits
```

### 3. تطبيق Migration على قاعدة البيانات

```bash
# تحديث قاعدة البيانات
dotnet ef database update
```

### 4. إعادة تشغيل الخادم

بعد تطبيق Migration بنجاح، أعد تشغيل الخادم.

## التحقق:

بعد تطبيق Migration، يمكنك التحقق من وجود الجدول:
```sql
SELECT * FROM NursingUnits;
```

أو استخدام endpoint:
```
GET http://localhost:5230/api/NursingUnits
```

## ملاحظة:
إذا كنت تريد إضافة بيانات تجريبية، استخدم:
```
POST http://localhost:5230/api/NursingUnits/seed
```
(يتطلب تسجيل دخول كـ Admin)

