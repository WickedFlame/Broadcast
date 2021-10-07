
export class BroadcastStorage {
	constructor() {
		this.registerHandlers();
	}

	registerHandlers() {
		document.querySelector('.broadcast-storage').addEventListener('click',
			e => {
				if (e.target.closest('.broadcast-toggler-button')) {
					let elem = e.target.closest('.broadcast-toggler-section');
					elem.classList.toggle('is-open');
				}

				//if (e.target.closest('[data-value]')) {
				//	var elem = document.querySelector('#broadcast-storage-content');
				//	elem.innerHTML = e.target.getAttribute('data-value');
				//}
			});
	}
}