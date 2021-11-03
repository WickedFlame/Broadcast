import { BroadcastBase } from './broadcast-base.js';

export class BroadcastStorage extends BroadcastBase {
	constructor(config) {
		super();

		this.config = config;
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

if (storageConfig === undefined) {
	storageConfig = {
		//TODO: /broadcast/ has to be able to be appended as configuration
		pollUrl: "/broadcast/storage",
		pollInterval: 2000
	};
}

const storage = new BroadcastStorage(storageConfig);