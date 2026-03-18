# تطبيق Migration لجدول NursingUnits

## ⚠️ مهم: يجب إيقاف الخادم أولاً!

## الخطوات:

### 1. إيقاف الخادم (Backend)
أوقف تشغيل الخادم أولاً قبل تطبيق Migration.

### 2. تطبيق Migration على قاعدة البيانات

افتح Terminal في مجلد المشروع:
```bash
cd "Hospital Mangement System\Hospital Mangement System"

# تطبيق Migration على قاعدة البيانات
dotnet ef database update
```

إذا ظهرت رسالة خطأ تفيد بأن Migration موجود بالفعل، استخدم:
```bash
dotnet ef database update 20250120120000_AddNursingUnits
```

### 3. إعادة تشغيل الخادم

بعد تطبيق Migration بنجاح، أعد تشغيل الخادم.

### 4. التحقق

بعد إعادة التشغيل، استخدم:
```
GET http://localhost:5230/api/NursingUnits/check-table
```

يجب أن ترى:
```json
{
  "tableExists": true,
  "recordCount": 0,
  "message": "NursingUnits table exists and is accessible"
}
```

### 5. (اختياري) إضافة بيانات تجريبية

بعد تسجيل الدخول كـ Admin:
```
POST http://localhost:5230/api/NursingUnits/seed
```

سيتم إضافة 12 وحدة تمريض تجريبية.

## ملاحظات:

- ✅ تم إنشاء ملفات Migration بالفعل في:
  - `Migrations/20250120120000_AddNursingUnits.cs`
  - `Migrations/20250120120000_AddNursingUnits.Designer.cs`

- ⚠️ يجب إيقاف الخادم قبل تطبيق Migration

- ✅ بعد التطبيق، سيتم إنشاء جدول `NursingUnits` في قاعدة البيانات

