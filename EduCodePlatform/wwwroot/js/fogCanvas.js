/* fogCanvas.js — "дим" з частинок */
console.log("fogCanvas.js завантажено!");

const fogCanvas = document.getElementById('fog-canvas');
if (fogCanvas) {
    const fogCtx = fogCanvas.getContext('2d');

    const particles = [];
    const particleCount = 200; // Збільшена кількість
    const maxOpacity = 1;      // Максимальна непрозорість

    class Particle {
        constructor() {
            // Випадкове розміщення
            this.x = Math.random() * fogCanvas.width;
            this.y = Math.random() * fogCanvas.height;
            // Нехай радіус буде менший, щоб частинок було більше й вони були помітні
            this.radius = Math.random() * 20 + 10; 
            // Випадкова прозорість у межах maxOpacity
            this.opacity = Math.random() * maxOpacity;
            // Невелика швидкість
            this.speedX = Math.random() * 0.7 - 0.35;
            this.speedY = Math.random() * 0.7 - 0.35;
        }

        draw() {
            fogCtx.beginPath();
            fogCtx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);

            // Тимчасово робимо колір червоним, щоб перевірити, чи ми бачимо частинки
            // Потім можна повернути на білий.
            const gradient = fogCtx.createRadialGradient(
                this.x, this.y, this.radius * 0.3,
                this.x, this.y, this.radius
            );
            // Змінний колір для наочності (білий у центрі -> прозорий на краях)
            gradient.addColorStop(0, `rgba(255, 255, 255, ${this.opacity})`);
            gradient.addColorStop(1, `rgba(255, 255, 255, 0)`);

            fogCtx.fillStyle = gradient;
            fogCtx.fill();
            fogCtx.closePath();
        }

        update() {
            this.x += this.speedX;
            this.y += this.speedY;

            // Якщо частинка виходить за межі, переносимо її на протилежну сторону
            if (this.x - this.radius > fogCanvas.width)  this.x = -this.radius;
            if (this.x + this.radius < 0)                this.x = fogCanvas.width + this.radius;
            if (this.y - this.radius > fogCanvas.height) this.y = -this.radius;
            if (this.y + this.radius < 0)                this.y = fogCanvas.height + this.radius;
        }
    }

    function initParticles() {
        particles.length = 0;
        // Якщо хочете фіксовану кількість, можна просто використати particleCount без "densityFactor"
        const densityFactor = Math.max(1, fogCanvas.width / 800);
        const dynamicCount = Math.floor(particleCount * densityFactor);

        for (let i = 0; i < dynamicCount; i++) {
            particles.push(new Particle());
        }
    }

    function resizeCanvas() {
        fogCanvas.width = window.innerWidth;
        fogCanvas.height = window.innerHeight;
        initParticles();
    }

    function animateFog() {
        // Замість заливки чорним - очищуємо попередній кадр
        fogCtx.clearRect(0, 0, fogCanvas.width, fogCanvas.height);

        // Малюємо всі частинки
        particles.forEach(p => {
            p.update();
            p.draw();
        });
        requestAnimationFrame(animateFog);
    }

    // Обробник зміни розміру вікна
    window.addEventListener('resize', resizeCanvas);

    // Старт
    resizeCanvas();
    animateFog();
} else {
    console.error("Не знайдено canvas з id='fog-canvas'.");
}
