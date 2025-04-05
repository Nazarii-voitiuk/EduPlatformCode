# EduPlatformCode
# 🧠 EduCodePlatform — Інтерактивна освітня платформа для практики програмування

![ASP.NET Core](https://img.shields.io/badge/Framework-ASP.NET_Core-blue)
![Entity Framework](https://img.shields.io/badge/ORM-Entity_Framework_Core-green)
![License](https://img.shields.io/badge/License-MIT-lightgrey)
![Status](https://img.shields.io/badge/Status-Active-brightgreen)

---

## 🎯 Проєкт

**EduCodePlatform** — це веб-платформа для створення, редагування, збереження та публікації фрагментів HTML/CSS/JavaScript-коду, орієнтована на навчальний процес. Система підтримує авторизацію, ролі, публічну галерею, історію змін, онлайн-редактор та адміністративну панель.

---

## ⚙️ Функціонал

### 👤 Для користувача:
- ✅ Реєстрація/вхід через email та Google OAuth
- 📝 Онлайн-редактор з підтримкою HTML/CSS/JS (CodeMirror)
- 🌌 Галерея публічних робіт
- ♻️ Історія змін і попередні версії коду
- 🛠️ Налаштування теми, табуляції, відображення

### 🛡️ Для адміністратора:
- 👥 Перегляд усіх submissions
- ⚙️ Управління списком мов програмування
- 🧩 Вибір власника для коду
- 🧾 Перегляд налаштувань користувачів

---

## 🏗️ Технології

| Технологія | Призначення |
|------------|-------------|
| **ASP.NET Core MVC** | Фреймворк для побудови бекенду |
| **Entity Framework Core** | Робота з базою даних |
| **CodeMirror** | Візуальний редактор коду |
| **Identity + Google OAuth2** | Автентифікація та ролі |
| **Bootstrap 5** | UI/UX дизайн |
| **jQuery + AJAX** | Клієнт-серверна взаємодія |
| **SQL Server** | Система управління базою даних |

---

## 🔐 Безпека

- Захист від XSS, CSRF, SQL Injection
- Хешування паролів через ASP.NET Identity
- Гнучке SMTP для безпечного скидання паролю

---

## 🚀 Як запустити

```bash
git clone https://github.com/Nazarii-voitiuk/EduPlatformCode.git
cd EduCodePlatform

