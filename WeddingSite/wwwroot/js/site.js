function initializeCountdown() {
    const targetDate = new Date("Sat Jul 05 2025 15:00:00 GMT+0200 (Central European Summer Time)");
    const countDownElement = document.querySelector('.wedding-countdown');

    let interval;

    function updateCountdown() {
        const currentDate = new Date();
        const diff = targetDate - currentDate;
        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((diff % (1000 * 60)) / 1000);

        if (days <= 0 && hours <= 0 && minutes <= 0 && seconds <= 0) {
            countDownElement.innerHTML = "";
            clearInterval(interval);
            return;
        }

        let timeLeft = [];
        if (days > 0) {
            timeLeft.push(`${days} ${days != 1 ? "dagar" : "dag"}`);
        }

        if (hours > 0) {
            timeLeft.push(`${hours} ${hours != 1 ? "timmar" : "timme"}`);
        }

        if (minutes > 0) {
            timeLeft.push(`${minutes} ${minutes != 1 ? "minuter" : "minut"}`);
        }

        countDownElement.innerHTML = timeLeft.join(", ") + " kvar";
    }

    updateCountdown();

    interval = setInterval(updateCountdown, 5000);
}

initializeCountdown();