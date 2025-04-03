let editMode = false;

$(document).ready(function () {
    loadLanguages();
});

function loadLanguages() {
    $.ajax({
        url: '/ProgrammingLanguage/GetAll',
        type: 'GET',
        success: function (data) {
            let tbody = $('#languageTable tbody');
            tbody.empty();

            data.forEach(function (lang) {
                let row = `
                    <tr>
                        <td>${lang.languageId}</td>
                        <td>${lang.name}</td>
                        <td>
                            <button class="btn btn-sm btn-info" 
                                onclick="openEditModal(${lang.languageId}, '${lang.name}')">
                                Edit
                            </button>
                            <button class="btn btn-sm btn-danger"
                                onclick="deleteLanguage(${lang.languageId})">
                                Delete
                            </button>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
        },
        error: function (xhr) {
            alert("Error loading languages: " + xhr.responseText);
        }
    });
}

// Відкрити модальне вікно для створення
function openCreateModal() {
    editMode = false;
    $('#LanguageId').val('');
    $('#LanguageName').val('');
    $('#languageModalLabel').text('Create New Language');
    $('#saveBtn').text('Create');

    // Показати модалку (Bootstrap 5)
    const modalEl = document.getElementById('languageModal');
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
}

// Відкрити модальне вікно для редагування
function openEditModal(id, name) {
    editMode = true;
    $('#LanguageId').val(id);
    $('#LanguageName').val(name);
    $('#languageModalLabel').text('Edit Language');
    $('#saveBtn').text('Save Changes');

    const modalEl = document.getElementById('languageModal');
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
}

// Зберегти (Create / Edit)
function saveLanguage() {
    let id = $('#LanguageId').val();
    let name = $('#LanguageName').val();

    if (!name) {
        alert("Name is required!");
        return;
    }

    if (!editMode) {
        // CREATE
        $.ajax({
            url: '/ProgrammingLanguage/Create',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Name: name }),
            success: function (res) {
                alert(res.message);
                closeModal();
                loadLanguages();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        // EDIT
        $.ajax({
            url: '/ProgrammingLanguage/Edit',
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                LanguageId: parseInt(id),
                Name: name
            }),
            success: function (res) {
                alert(res.message);
                closeModal();
                loadLanguages();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    }
}

// Закрити модальне вікно вручну (якщо треба)
function closeModal() {
    const modalEl = document.getElementById('languageModal');
    const modal = bootstrap.Modal.getInstance(modalEl);
    if (modal) {
        modal.hide();
    }
}

// Видалити
function deleteLanguage(id) {
    if (!confirm("Are you sure you want to delete this language?")) return;

    $.ajax({
        url: '/ProgrammingLanguage/Delete/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadLanguages();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}
