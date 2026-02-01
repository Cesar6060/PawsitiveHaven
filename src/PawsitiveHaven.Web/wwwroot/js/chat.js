// Chat helper functions for Blazor interop

window.chatHelpers = {
    scrollToBottom: function (selector) {
        const element = document.querySelector(selector);
        if (element) {
            element.scrollTo(0, element.scrollHeight);
        }
    },

    focusElement: function (selector) {
        const element = document.querySelector(selector);
        if (element) {
            element.focus();
        }
    }
};
