(function () {
    function addHomeButton() {
        const info = document.querySelector(
            '.swagger-ui .information-container'
        );

        if (!info || document.querySelector('.swagger-home-button')) {
            return false;
        }

        const button = document.createElement('a');
        button.href = '/';
        button.className = 'swagger-home-button';
        button.textContent = '← Back to Home';

        button.style.cssText = `
            display: inline-block;
            margin: 12px 0 20px;
            padding: 11px 20px;
            background: #2563eb;
            color: white;
            border-radius: 9px;
            text-decoration: none;
            font-size: 14px;
            font-weight: bold;
        `;

        button.style.display = 'block';
        button.style.width = 'fit-content';
        button.style.margin = '12px 20px 20px auto';

        info.parentNode.insertBefore(button, info);
        return true;
    }

    const observer = new MutationObserver(() => {
        if (addHomeButton()) {
            observer.disconnect();
        }
    });

    observer.observe(document.documentElement, {
        childList: true,
        subtree: true
    });

    addHomeButton();
})();