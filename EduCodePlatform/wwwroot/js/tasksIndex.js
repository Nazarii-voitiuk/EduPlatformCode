let editMode = false;

$(document).ready(function () {
    loadTasks();
});

function loadTasks() {
    $.ajax({
        url: '/Tasks/GetAll',
        type: 'GET',
        success: function (data) {
            let tbody = $('#tasksTable tbody');
            tbody.empty();
            data.forEach(item => {
                let row = `
                <tr>
                    <td>${item.taskId}</td>
                    <td>${item.title}</td>
                    <td>${item.difficultyName || ''}</td>
                    <td>${formatDate(item.createdAt)}</td>
                    <td>
                        <button class="btn btn-sm btn-success" onclick="goSolve(${item.taskId})">Solve</button>
                        <button class="btn btn-sm btn-warning" onclick="goManageTestCases(${item.taskId})">TestCases</button>
                        <button class="btn btn-sm btn-info" onclick="openEditModal(${item.taskId})">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteTask(${item.taskId})">Delete</button>
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

function openCreateModal() {
    editMode = false;
    clearModal();
    $('#taskModalLabel').text("Create Task");
    $('#saveBtn').text("Create");
    loadDifficultiesIntoSelect(0);

    // Відкрити (Bootstrap 5)
    const modalEl = document.getElementById('taskModal');
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
}

function openEditModal(id) {
    editMode = true;
    clearModal();

    $.ajax({
        url: `/Tasks/GetById/${id}`,
        type: 'GET',
        success: function (task) {
            $('#TaskId').val(task.taskId);
            $('#Title').val(task.title);
            $('#Description').val(task.description || '');
            $('#IsAIgenerated').prop('checked', task.isAIgenerated === true);

            $('#ReferenceHtml').val(task.referenceHtml || '');
            $('#ReferenceCss').val(task.referenceCss || '');
            $('#ReferenceJs').val(task.referenceJs || '');

            loadDifficultiesIntoSelect(task.difficultyId);

            $('#taskModalLabel').text("Edit Task (#" + task.taskId + ")");
            $('#saveBtn').text("Save Changes");

            const modalEl = document.getElementById('taskModal');
            const modal = new bootstrap.Modal(modalEl);
            modal.show();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function clearModal() {
    $('#TaskId').val('');
    $('#Title').val('');
    $('#Description').val('');
    $('#IsAIgenerated').prop('checked', false);

    $('#ReferenceHtml').val('');
    $('#ReferenceCss').val('');
    $('#ReferenceJs').val('');

    $('#DifficultySelect').empty();
}

function loadDifficultiesIntoSelect(selectedId) {
    $.ajax({
        url: '/TaskDifficulty/GetAll',
        type: 'GET',
        success: function (data) {
            let sel = $('#DifficultySelect');
            sel.empty();
            sel.append(`<option value="0">--Select--</option>`);
            data.forEach(d => {
                sel.append(`<option value="${d.difficultyId}">${d.difficultyName}</option>`);
            });
            if (selectedId > 0) {
                sel.val(selectedId);
            }
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function saveTask() {
    let id = $('#TaskId').val();
    let dataObj = {
        taskId: id ? parseInt(id) : 0,
        title: $('#Title').val(),
        description: $('#Description').val(),
        difficultyId: parseInt($('#DifficultySelect').val() || 0),
        isAIgenerated: $('#IsAIgenerated').is(':checked'),
        referenceHtml: $('#ReferenceHtml').val(),
        referenceCss: $('#ReferenceCss').val(),
        referenceJs: $('#ReferenceJs').val()
    };

    if (!editMode) {
        $.ajax({
            url: '/Tasks/CreateAjax',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                closeTaskModal();
                loadTasks();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        $.ajax({
            url: '/Tasks/EditAjax',
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                closeTaskModal();
                loadTasks();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    }
}

function closeTaskModal() {
    const modalEl = document.getElementById('taskModal');
    const modal = bootstrap.Modal.getInstance(modalEl);
    if (modal) {
        modal.hide();
    }
}

function deleteTask(id) {
    if (!confirm("Delete task #" + id + "?")) return;
    $.ajax({
        url: '/Tasks/DeleteAjax/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadTasks();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

// Додаткові кнопки:
function goSolve(id) {
    window.location.href = '/Tasks/Solve?taskId=' + id;
}
function goManageTestCases(id) {
    window.location.href = '/Tasks/ManageTestCases?taskId=' + id;
}

function formatDate(str) {
    if (!str) return "";
    let d = new Date(str);
    return d.toLocaleString();
}
