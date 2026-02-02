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
    },

    autoResizeTextarea: function (selector) {
        const textarea = document.querySelector(selector);
        if (textarea) {
            textarea.style.height = 'auto';
            textarea.style.height = Math.min(textarea.scrollHeight, 96) + 'px';
        }
    },

    resetTextareaHeight: function (selector) {
        const textarea = document.querySelector(selector);
        if (textarea) {
            textarea.style.height = 'auto';
        }
    }
};
