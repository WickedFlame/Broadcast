import { BroadcastBase } from './broadcast-base';

export class BroadcastStorage extends BroadcastBase {
	constructor(config) {
		super();

		this.config = config;
		this.registerHandlers();

		this.startPolling(config, (data) => this.updateDashboard(data, this));
	}

	registerHandlers() {
		document.querySelector('.broadcast-storage').addEventListener('click',
			e => {
				if (e.target.closest('.broadcast-toggler-section')) {
					let elem = e.target;
					elem.classList.toggle('is-open');
				}
			});
	}

	updateDashboard(data, broadcast) {
		data.forEach(type => {
			if (type.key === 'Servers') {
				var list = document.querySelector('#serverlist');
				type.items.forEach(item => {
					var key = item.key.replace(new RegExp(':', 'g'), '_').replace(new RegExp(' ', 'g'), '_');
					var elem = list.querySelector(`#server-${key}`);
					if (elem) {
						this.updateStorageElement(elem, item.values);
					} else {
						var child = this.createStorageElement(`server-${key}`, item.key, item.values);
						this.appendChild(list, child);
					}
				});
			} else if (type.key === 'Tasks') {
				var list = document.querySelector('#tasklist');
				type.items.forEach(item => {
					var key = item.key.replace(new RegExp(':', 'g'), '_').replace(new RegExp(' ', 'g'), '_');
					var elem = list.querySelector(`#tasks-${key}`);
					if (elem) {
						if (elem.getAttribute('data-state') !== 'Processed') {
							this.updateStorageElement(elem, item.values);
						}
					} else {
						var child = this.createStorageElement(`tasks-${key}`, item.key, item.values);
						child.setAttribute('data-state', item.values.find(i => i.key === 'State').value);
						this.appendChild(list, child);
					}
				});
			}else if (type.key === 'Recurring') {
				var list = document.querySelector('#recurringlist');
				type.items.forEach(item => {
					var key = item.key.replace(new RegExp(':', 'g'), '_').replace(new RegExp(' ', 'g'), '_');
					var elem = list.querySelector(`#recurring-${key}`);
					if (elem) {
						this.updateStorageElement(elem, item.values);
					} else {
						var child = this.createStorageElement(`recurring-${key}`, item.key, item.values);
						this.appendChild(list, child);
					}
				});
			}else if (type.key === 'Queues') {
				var list = document.querySelector('#queuelist');
				type.items.forEach(item => {
					var key = item.key.replace(new RegExp(':', 'g'), '_').replace(new RegExp(' ', 'g'), '_');
					var elem = list.querySelector(`#queue-${key}`);
					if (elem) {
						this.updateStorageElement(elem, item.values);
					} else {
						var child = this.createStorageElement(`queue-${key}`, item.key, item.values);
						this.appendChild(list, child);
					}
				});
			}
		});
	}

	createStorageElement(key, title, values) {
		var html = `<div class="broadcast-row" id="${key}"><div class="broadcast-table-row broadcast-storage-header broadcast-toggler-section">${title}</div><div class="broadcast-storage-content broadcast-toggle-wrapper">
${this.createStorageValues(values)}
</div></div>`;
		var div = document.createElement("div");
		div.innerHTML = html;
		return div.firstChild;
	}

	createStorageValues(values) {
		var html = '';

		values.forEach(value => {
			if (value.key) {
				html += `<div class="broadcast-storage-row"><div class="broadcast-storage-key">${value.key}</div><div class="broadcast-storage-value">${value.value}</div></div>`;
			} else {
				html += `<div class="broadcast-storage-row-single">${value.value}</div>`;
			}
		});

		return html;
	}

	updateStorageElement(elem, values) {
		elem.querySelector('.broadcast-storage-content').innerHTML = this.createStorageValues(values);
		var state = values.find(i => i.key === 'State');
		if (state) {
			elem.setAttribute('data-state', state.value);
		}
	}

	appendChild(list, child) {
		var row = list.querySelector('.broadcast-row');
		if (row) {
			list.insertBefore(child, row);
		} 
		else {
			list.appendChild(child);
		}
	}
}

if (storageConfig === undefined) {
	var storageConfig = {
		pollUrl: "%(RouteBasePath)/storage/keys",
		pollInterval: 2000
	};
}

const storage = new BroadcastStorage(storageConfig);