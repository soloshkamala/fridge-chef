
window.fridgeAuth = {
    save: function (email) {
        sessionStorage.setItem("fridgeUserEmail", email);
    },

    get: function () {
        return sessionStorage.getItem("fridgeUserEmail");
    },

    clear: function () {
        sessionStorage.removeItem("fridgeUserEmail");
    }
};
