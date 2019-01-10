class Particle {
	size = 0.5 + Math.random() * 3.5;
	position = {x: 0, y: 0};
	shift = {x: 0, y: 0};
	angle = 0;
	speed = 0.01 + this.size / 4 * 0.03;
	force = 1 - Math.random() * 0.11;
	color = "#ffffff";
	orbit = 1;
	magnet = null;
}

class Magnet {
	orbit = 100;
	position = {x: 0, y: 0};
	connections = 0;
	size = 1;
	speed = 0.04;
}

export class Magnetic {

	cx: number = 600;
	cy: number = 800;
	scale: number = 0.002;
	scaleStep: number = 0;

	magnetCount: number = 6;
	magnetConst: number = 500;
	div: any;
	drawContext: any;

	particlesToMagnet: number = 30;
	particles: Particle[] = [];
	magnets: Magnet[] = [];

	theme = {
		glowA: "rgba(0,0,0,0.3)"
		, glowB: "rgba(0,0,0,0.0)"
		, particleFill: "#333333"
		, fadeFill: "rgba(22,22,22,.6)"
		, useFade: false
	};

	addMagnet = (a) => {
		var magnet = new Magnet();
		magnet.position.x = a.x;
		magnet.position.y = a.y;
		this.magnets.push(magnet);
		for (let i = 0; i < this.particlesToMagnet; i++) {
			var c = new Particle();
			c.position.x = a.x;
			c.position.y = a.y;
			c.shift.x = a.x;
			c.shift.y = a.y;
			c.color = this.theme.particleFill;
			this.particles.push(c);
		}
	};

	resize = () => {
		var dx = (window.innerWidth - this.cx) / 2;
		var dy = (window.innerHeight - this.cy) / 2;

		for (var a = 0; a < this.magnetCount; a++) {
			this.magnets[a].position.x += dx;
			this.magnets[a].position.y += dy;
		}
		this.cx = window.innerWidth;
		this.cy = window.innerHeight;
		this.div.width = this.cx;
		this.div.height = this.cy;
		this.drawContext = this.div.getContext("2d");
	};

	tick = () => {
		this.div.onresize = this.resize;

		if (this.theme.useFade) {
			this.drawContext.fillStyle = this.theme.fadeFill;
			this.drawContext.fillRect(0, 0, this.drawContext.canvas.width, this.drawContext.canvas.height);
		} else
			this.drawContext.clearRect(0, 0, this.div.width, this.div.height);

		// Вычисляем центр фигуры
		let magnetsLenght = this.magnets.length;
		let size = {minX: 999999, minY: 999999, maxX: -999999, maxY: -999999};
		for (let i = 0; i < magnetsLenght; i++) {
			let magnet = this.magnets[i];
			size.minX = Math.min(size.minX, magnet.position.x);
			size.minY = Math.min(size.minY, magnet.position.y);
			size.maxX = Math.max(size.maxX, magnet.position.x);
			size.maxY = Math.max(size.maxY, magnet.position.y);
		}

		let center = {x: (size.minX + size.maxX) / 2, y: (size.minY + size.maxY) / 2};

		// Корректируем центр, а то из-за округления он убегает
		center.x = Math.max(Math.min(center.x, this.cx * 3 / 4), this.cx / 4);
		center.y = Math.max(Math.min(center.y, this.cy * 3 / 4), this.cy / 4);

		this.scaleStep++;
		if (this.scaleStep > 300) {
			this.scale = -this.scale;
			this.scaleStep = 0;
		}

		for (let i = 0; i < magnetsLenght; i++) {

			let magnet = this.magnets[i];

			// перемещаем магниты против часовой стрелки относительоно центра группы
			let dist = Math.abs(this.distanceBetween(center, magnet.position)) * (1 + this.scale);
			let angle = this.corner(center, magnet.position) - (magnet.speed / (magnet.connections + 1));
			magnet.position.x = center.x + Math.cos(angle) * dist;
			magnet.position.y = center.y + Math.sin(angle) * dist;


			// Размер магнита зависит от количества присоедин   енных частиц
			magnet.size += (magnet.connections / 3 - magnet.size) * 0.05;

			// Размер магнита не может быть меньше 2
			magnet.size = Math.max(magnet.size, 2);

			// Отрисовываем магнит
			let object = this.drawContext.createRadialGradient(magnet.position.x, magnet.position.y, 0, magnet.position.x, magnet.position.y, magnet.size * 10);
			object.addColorStop(0, this.theme.glowA);
			object.addColorStop(1, this.theme.glowB);
			this.drawContext.beginPath();
			this.drawContext.fillStyle = object;
			this.drawContext.arc(magnet.position.x, magnet.position.y, magnet.size * 10, 0, Math.PI * 2, true);
			this.drawContext.fill();
			this.drawContext.beginPath();
			this.drawContext.fillStyle = object;
			this.drawContext.arc(magnet.position.x, magnet.position.y, magnet.size, 0, Math.PI * 2, true);
			this.drawContext.fill();
			magnet.connections = 0;
		}

		let particlesLenght = this.particles.length;
		for (let j = 0; j < particlesLenght; j++) {

			let particle = this.particles[j];
			let minDist = -1;
			let nearMagnet = null;
			let v = {x: 0, y: 0};

			for (let i = 0; i < magnetsLenght; i++) {
				let magnet = this.magnets[i];

				// Корректируем позицию частицы в зависимости о магнитов
				if (particle.magnet != magnet) {
					let x = magnet.position.x - particle.position.x;
					if (Math.abs(x) < this.magnetConst)
						v.x += x / this.magnetConst;

					let y = magnet.position.y - particle.position.y;
					if (Math.abs(y) < this.magnetConst)
						v.y += y / this.magnetConst;
				}

				// Определяем для частицы самый ближний магнит
				let dist = Math.abs(this.distanceBetween(particle.position, magnet.position));
				if (nearMagnet == null || minDist > dist) {
					minDist = dist;
					nearMagnet = magnet;
				}
			}

			particle.magnet = nearMagnet;
			nearMagnet.connections += 1;

			particle.angle += particle.speed;
			particle.shift.x += (nearMagnet.position.x + v.x * 10 - particle.shift.x) * particle.speed;
			particle.shift.y += (nearMagnet.position.y + v.y * 10 - particle.shift.y) * particle.speed;
			particle.position.x = particle.shift.x + Math.cos(j + particle.angle) * particle.orbit * particle.force;
			particle.position.y = particle.shift.y + Math.sin(j + particle.angle) * particle.orbit * particle.force;
			particle.position.x = Math.max(Math.min(particle.position.x, this.cx - particle.size / 2), particle.size / 2);
			particle.position.y = Math.max(Math.min(particle.position.y, this.cy - particle.size / 2), particle.size / 2);
			particle.orbit += (nearMagnet.orbit - particle.orbit) * 0.1;
			this.drawContext.beginPath();
			this.drawContext.fillStyle = particle.color;
			this.drawContext.arc(particle.position.x, particle.position.y, particle.size / 2, 0, Math.PI * 2, true);
			this.drawContext.fill();
		}
	};

	distanceBetween = (a, b) => {
		let c = b.x - a.x, h = b.y - a.y;
		return Math.sqrt(c * c + h * h);
	};

	corner = (a, b) => {
		let c = b.x - a.x, h = b.y - a.y;
		if (c == 0) {
			if (h > 0)
				return Math.PI / 2;
			else
				return -Math.PI / 2;
		}

		if (h == 0) {
			if (c > 0)
				return 0;
			else
				return -Math.PI;
		}

		if (c > 0)
			return Math.atan(h / c);
		else
			return Math.atan(h / c) - Math.PI
	};

	init = () => {
		this.div = document.getElementById("drawCanvas");

		if (this.div && this.div.getContext) {

			this.cx = window.innerWidth;
			this.cy = window.innerHeight;
			this.div.width = this.cx;
			this.div.height = this.cy;

			this.drawContext = this.div.getContext("2d");

			window.addEventListener("resize", this.resize);

			for (var a = 0; a < this.magnetCount; a++) {
				this.addMagnet({
					x: (this.cx - 300) * 0.5 + Math.random() * 300,
					y: (this.cy - 300) * 0.5 + Math.random() * 300
				});
			}
		}
	};
}
