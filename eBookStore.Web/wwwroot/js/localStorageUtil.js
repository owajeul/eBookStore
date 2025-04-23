
const localStorageUtil = {
    saveToLocalStorage: function (key, data) {
        const obj = {
            data: data,
            timestamp: new Date().getTime()
        };
        localStorage.setItem(key, JSON.stringify(obj));
    },
    getFromLocalStorage: function (key, maximumTimeInMinutes) {
        const item = localStorage.getItem(key);
        if (!item) return null;

        try {
            const parsed = JSON.parse(item);
            const now = new Date().getTime();
            const elapsedTimeInMinutes = (now - parsed.timestamp) / (1000 * 60);

            if (elapsedTimeInMinutes > maximumTimeInMinutes) {
                localStorage.removeItem(key);
                return null;
            }
            return parsed.data;
        } catch (error) {
            console.error("Failed to parse localStorage data", error);
            localStorage.removeItem(key);
            return null;
        }
    }
};

window.localStorageUtil = localStorageUtil;
