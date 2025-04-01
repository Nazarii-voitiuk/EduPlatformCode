let editMode = false;

$(document).ready(function () {
    loadSubmissions();
    loadLanguagesIntoSelect();
});

// 1. Завантаження списку CodeSubmissions
function loadSubmissions() {
    $.ajax({
        url: '/CodeSubmissions/GetAll',
        type: 'GET',
        success: function (data) {
            let tbody = $('#codeTable tbody');
            tbody.empty();

            data.forEach(function (item) {
                let row = `<tr>
                    <td>${item.codeSubmissionId}</td>
                    <td>${item.userId || ''}</td>
                    <td>${item.languageName || ''}</td>
                    <td>${formatDate(item.createdAt)}</td>
                    <td>${formatDate(item.updatedAt)}</td>
                    <td>
                        <button class="btn btn-sm btn-info" 
                            onclick="openEditModal(${item.codeSubmissionId}, '${item.userId}', '${item.languageName}', \`${escapeString(item.codeText)}\`)">
                            Edit
                        </button>
                        <button class="btn btn-sm btn-danger" 
                            onclick="deleteSubmission(${item.codeSubmissionId})">
                            Delete
                        </button>
                    </td>
                </tr>`;
                tbody.append(row);
            });
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

// 2. Відкрити модальне вікно для створення
function openCreateModal() {
    editMode = false;
    clearModal();
    $('#codeModalLabel').text('Create New Submission');
    $('#saveBtn').text('Create');
    $('#codeModal').modal('show');
}

// 3. Відкрити модальне вікно для редагування
function openEditModal(id, userId, langName, code) {
    editMode = true;
    clearModal();

    $('#SubmissionId').val(id);
    $('#UserId').val(userId);

    // Оскільки у нас LanguageId, а з сервера прийшла languageName,
    // треба буде знайти LanguageId, або зберігати його теж на клієнті.
    // Для спрощення - хай користувач знову обирає
    // (або шукаємо в <select> за назвою).
    // Можна заздалегідь отримувати LanguageId у GetAll(), 
    // але тут ми робимо примітивний варіант.

    $('#CodeText').val(code);

    $('#codeModalLabel').text('Edit Submission');
    $('#saveBtn').text('Save Changes');
    $('#codeModal').modal('show');
}

// 4. Закрити/очистити
function closeModal() {
    $('#codeModal').modal('hide');
}
function clearModal() {
    $('#SubmissionId').val('');
    $('#UserId').val('');
    $('#LanguageId').val('');
    $('#CodeText').val('');
}

// 5. Зберегти (Create / Edit)
function saveSubmission() {
    let id = $('#SubmissionId').val();
    let userId = $('#UserId').val();
    let languageId = $('#LanguageId').val();
    let codeText = $('#CodeText').val();

    // Валідація
    if (!languageId) {
        alert("Please select a language.");
        return;
    }

    if (!editMode) {
        // CREATE
        let dataObj = {
            UserId: userId,
            LanguageId: parseInt(languageId),
            CodeText: codeText
        };

        $.ajax({
            url: '/CodeSubmissions/Create',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                closeModal();
                loadSubmissions();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        // EDIT
        let dataObj = {
            CodeSubmissionId: parseInt(id),
            UserId: userId,
            LanguageId: parseInt(languageId),
            CodeText: codeText
        };

        $.ajax({
            url: '/CodeSubmissions/Edit',
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                closeModal();
                loadSubmissions();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    }
}

// 6. Видалити
function deleteSubmission(id) {
    if (!confirm("Are you sure to delete this submission?")) return;

    $.ajax({
        url: '/CodeSubmissions/Delete/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadSubmissions();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

// 7. Завантажити список мов у <select>
function loadLanguagesIntoSelect() {
    $.ajax({
        url: '/ProgrammingLanguage/GetAll',
        type: 'GET',
        success: function (data) {
            let sel = $('#LanguageId');
            sel.empty();
            sel.append(`<option value="">-- Select Language --</option>`);

            data.forEach(function (lang) {
                sel.append(`<option value="${lang.languageId}">${lang.name}</option>`);
            });
        },
        error: function (xhr) {
            alert("Error loading languages: " + xhr.responseText);
        }
    });
}

// Допоміжна функція форматування дати
function formatDate(str) {
    if (!str) return "";
    // str = "2025-03-18T10:20:00"
    let date = new Date(str);
    return date.toLocaleString();
}

// Допоміжна функція екранування рядків (щоб вставити у template-літерал)
function escapeString(str) {
    if (!str) return "";
    return str.replace(/`/g, "\\`").replace(/\$/g, "\\$");
}
