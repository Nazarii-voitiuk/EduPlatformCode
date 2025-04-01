// === Плаваючий курсор-дракон ===
const cursorCanvas = document.getElementById('dragonCanvas');
if (cursorCanvas) {
    const cursorCtx = cursorCanvas.getContext('2d');

    function resizeCursorCanvas() {
        cursorCanvas.width = window.innerWidth;
        cursorCanvas.height = window.innerHeight;
    }

    resizeCursorCanvas();
    window.addEventListener('resize', resizeCursorCanvas);

    const dragon = {
        x: cursorCanvas.width / 2,
        y: cursorCanvas.height / 2,
        size: 20,
        angle: 0,
        speed: 0.05
    };

    const trails = [];

    function drawDragon(x, y, size, angle) {
        cursorCtx.save();
        cursorCtx.translate(x, y);
        cursorCtx.rotate(angle);
        cursorCtx.beginPath();
        cursorCtx.moveTo(-size, size);
        cursorCtx.lineTo(size, 0);
        cursorCtx.lineTo(-size, -size);
        cursorCtx.closePath();
        cursorCtx.fillStyle = 'white';
        cursorCtx.fill();
        cursorCtx.restore();
    }

    function drawTrail() {
        cursorCtx.clearRect(0, 0, cursorCanvas.width, cursorCanvas.height);

        for (let i = trails.length - 1; i >= 0; i--) {
            const trail = trails[i];

            cursorCtx.save();
            cursorCtx.translate(trail.x, trail.y);
            cursorCtx.rotate(trail.angle);
            cursorCtx.globalAlpha = trail.alpha;

            cursorCtx.beginPath();
            cursorCtx.moveTo(-trail.size, trail.size);
            cursorCtx.lineTo(trail.size, 0);
            cursorCtx.lineTo(-trail.size, -trail.size);
            cursorCtx.closePath();
            cursorCtx.fillStyle = 'rgba(255, 255, 255, 0.5)';
            cursorCtx.fill();

            cursorCtx.restore();

            trail.alpha -= 0.02;
            trail.size *= 0.98;

            if (trail.alpha <= 0) {
                trails.splice(i, 1);
            }
        }
    }

    function updateDragon() {
        const dx = targetX - dragon.x;
        const dy = targetY - dragon.y;
        const distance = Math.sqrt(dx * dx + dy * dy);
        dragon.angle = Math.atan2(dy, dx);

        if (distance > 1) {
            dragon.x += dx * dragon.speed;
            dragon.y += dy * dragon.speed;
        }

        trails.push({
            x: dragon.x,
            y: dragon.y,
            size: dragon.size,
            angle: dragon.angle,
            alpha: 1
        });
    }

    function animateCursor() {
        drawTrail();
        updateDragon();
        drawDragon(dragon.x, dragon.y, dragon.size, dragon.angle);
        requestAnimationFrame(animateCursor);
    }

    let targetX = cursorCanvas.width / 2;
    let targetY = cursorCanvas.height / 2;

    window.addEventListener('mousemove', (e) => {
        targetX = e.clientX;
        targetY = e.clientY;
    });

    animateCursor();
}
