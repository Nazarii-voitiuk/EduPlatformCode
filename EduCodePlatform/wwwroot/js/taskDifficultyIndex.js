let editMode = false;

$(document).ready(function () {
    loadDifficulties();
});

function loadDifficulties() {
    $.ajax({
        url: '/TaskDifficulty/GetAll',
        type: 'GET',
        success: function (data) {
            let tbody = $('#diffTable tbody');
            tbody.empty();
            data.forEach(d => {
                let row = `
                <tr>
                    <td>${d.difficultyId}</td>
                    <td>${d.difficultyName}</td>
                    <td>
                        <button class="btn btn-sm btn-info" onclick="openEditModal(${d.difficultyId}, '${escapeStr(d.difficultyName)}')">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteDifficulty(${d.difficultyId})">Delete</button>
                    </td>
                </tr>
                `;
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
    $('#diffModalLabel').text("Create Difficulty");
    $('#diffModal').modal('show');
}

function openEditModal(id, name) {
    editMode = true;
    clearModal();
    $('#DifficultyId').val(id);
    $('#DifficultyName').val(name);
    $('#diffModalLabel').text("Edit Difficulty #" + id);
    $('#diffModal').modal('show');
}

function clearModal() {
    $('#DifficultyId').val('');
    $('#DifficultyName').val('');
}

function saveDifficulty() {
    let id = $('#DifficultyId').val();
    let name = $('#DifficultyName').val();

    let dataObj = {
        difficultyId: id ? parseInt(id) : 0,
        difficultyName: name
    };

    if (!editMode) {
        // CREATE
        $.ajax({
            url: '/TaskDifficulty/CreateAjax',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                $('#diffModal').modal('hide');
                loadDifficulties();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        // EDIT
        $.ajax({
            url: '/TaskDifficulty/EditAjax',
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(dataObj),
            success: function (res) {
                alert(res.message);
                $('#diffModal').modal('hide');
                loadDifficulties();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    }
}

function deleteDifficulty(id) {
    if (!confirm("Delete difficulty #" + id + "?")) return;
    $.ajax({
        url: '/TaskDifficulty/DeleteAjax/' + id,
        type: 'DELETE',
        success: function (res) {
            alert(res.message);
            loadDifficulties();
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function escapeStr(str) {
    if (!str) return "";
    return str.replace(/`/g, "\\`").replace(/\$/g, "\\$");
}
