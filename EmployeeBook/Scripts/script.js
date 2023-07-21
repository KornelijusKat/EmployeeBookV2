function ActivateUserEdit(data) {
    console.log('hello');
    let stringValue = data;
    window.location.href = `/User/EditUser/?stringValue=${encodeURIComponent(stringValue)}`
}
function ActivateUserDelete(data) {
    console.log(data);
    fetch('/User/DeleteUser', {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            if (response.ok) {
                console.log('User deleted.');
                location.reload();
            } else {
                console.error('Failed to delete profile.');
            }
        })
        .catch(error => {
            console.error('An error occurred while deleting the User:', error);
        });
}
let DeleteButton = document.querySelector('#Delete');
DeleteButton.addEventListener('click', function (event) {
    console.log("hello");
})
function ActivateProfileEdit(data) {
    console.log('hello');
    let stringValue = data;
    window.location.href = `/Profile/EditPerson/?stringValue=${encodeURIComponent(stringValue)}`
}
function ActivateProfileDelete(data) {
    console.log(data);
    fetch('/Profile/DeleteProfile', {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            if (response.ok) {
                console.log('Profile deleted.');
                location.reload();
            } else {
                console.error('Failed to delete profile.');
            }
        })
        .catch(error => {
            console.error('An error occurred while deleting the profile:', error);
        });
} 