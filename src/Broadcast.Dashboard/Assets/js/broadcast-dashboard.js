import { BroadcastBase } from './broadcast-base';

export class BroadcastDashboard extends BroadcastBase {
	constructor(config) {
		super();

		this.config = config;
		//setTimeout(() => {
		//	this.startPolling(config, (data) => this.updateDashboard(data, this));
		//}, 3000);
		this.startPolling(config, (data) => this.updateDashboard(data, this));

		document.querySelector('#tasklist').addEventListener('click',
			e => {
				var trash = e.target.closest('.trash-button');
				if (trash) {
					this.deleteTask(`${config.dashboardUrl}/task/${trash.dataset.taskId}/delete`);
					return;
				}

				var row = e.target.closest('tr');
				if (row) {
					this.showDetail(`${config.dashboardUrl}/data/task/${row.dataset.taskId}`);
				}
			});

		document.querySelector('#recurringlist').addEventListener('click',
			e => {
				var trash = e.target.closest('.trash-button');
				if (trash) {
					this.deleteTask(`${config.dashboardUrl}/task/${trash.dataset.recurringId}/delete`);
					return;
				}

				var row = e.target.closest('tr');
				if (row) {
					this.showDetail(`${config.dashboardUrl}/data/recurringtask/${row.dataset.recurringId}`);
				}
			});

		document.querySelector('#serverlist').addEventListener('click',
			e => {
				var row = e.target.closest('tr');
				if (row) {
					this.showDetail(`${config.dashboardUrl}/data/server/${row.dataset.serverId}`);
				}
			});

		document.querySelector('#broadcast-overlay-close-btn').addEventListener('click',
			e => {
				e.target.closest('#broadcast-data-overlay').style.display = 'none';
			});
	}

	updateDashboard(data, dashboard) {
		// servers
		this.updateElement(document.querySelector('#broadcast-servers-count'), data.servers.length);
		var serverList = document.querySelector('#serverlist');
		data.servers.forEach(s => {
			var serverElem = serverList.querySelector(`#serverheartbeat_${s.id}`);
			if (serverElem) {
				this.updateElement(serverElem, this.formatDate(new Date(s.heartbeat)));
			} else {
				var row = serverList.querySelector('tbody').insertRow(0);
				row.setAttribute('data-server-id', s.id);
				row.classList.add('broadcast-table-row');

				this.addCell(row, 0, null, `${s.name}`);
				this.addCell(row, 1, null, `${s.id}`);
				this.addCell(row, 2, `serverheartbeat_${s.id}`, this.formatDate(new Date(s.heartbeat)));
			}
		});

		// recurring tasks
		this.updateElement(document.querySelector('#broadcast-recurring-count'), data.recurringTasks.length);
		var recurringlist = document.querySelector('#recurringlist');
		if (recurringlist) {
			data.recurringTasks.forEach(t => {
				var name = t.name.replace('.', '_');
				var taskRow = recurringlist.querySelector(`#recurring_${name}`);

				if (taskRow) {
					this.updateElement(taskRow.querySelector(`#referenceid_${name}`), t.referenceId);
					this.updateElement(taskRow.querySelector(`#nextexecution_${name}`), this.formatDate(new Date(t.nextExecution)));
				} else {
					// add new row
					var row = recurringlist.querySelector('tbody').insertRow(0);
					row.id = `recurring_${name}`;
					row.setAttribute('data-recurring-id', t.name);
					row.setAttribute('data-recurring-reference-id', t.referenceId);
					row.classList.add('broadcast-table-row');

					this.addCell(row, 0, null, t.name);
					this.addCell(row, 1, `referenceid_${name}`, t.referenceId);
					this.addCell(row, 2, `nextexecution_${name}`, this.formatDate(new Date(t.nextExecution)));
					this.addCell(row, 3, null, this.millisecondsToTime(t.interval));
					this.addCell(row, 4, null, '').innerHTML = `<div class="trash-box"><i class="trash-button" data-recurring-id="${t.referenceId}"></i></div>`;
				}
			});
		}

		// tasks
		var tasklist = document.querySelector('#tasklist');
		this.updateElement(document.querySelector('#broadcast-tasks-count'), data.tasks.length);
		var cnt = 0;
		var processedCnt = 0;
		var failedCnt = 0;
		data.tasks.forEach(t => {
			if (t.state !== 'processed' && t.state !== 'faulted' && t.state !== 'deleted') {
				cnt = cnt + 1;
			} else if (t.state === 'processed') {
				processedCnt = processedCnt + 1;
			} else if (t.state === 'faulted') {
				failedCnt = failedCnt + 1;
			}

			if (tasklist) {
				var taskRow = document.querySelector(`#task_${t.id}`);
				var state = t.state === 'new'
					? 'New'
					: t.state === 'queued'
					? 'Queued'
					: t.state === 'dequeued'
					? 'Dequeued'
					: t.state === 'processing'
					? 'Processing'
					: t.state === 'processed'
					? 'Processed'
					: t.state === 'faulted'
					? 'Faulted'
					: t.state === 'deleted'
					? 'Deleted'
					: 'Unknown';

				if (taskRow) {
					var stateElem = taskRow.querySelector(`#state_${t.id}`);
					if (stateElem.innerText !== state) {
						this.updateElement(stateElem, state);
						this.updateElement(taskRow.querySelector(`#server_${t.id}`), t.server);
						this.updateElement(taskRow.querySelector(`#start_${t.id}`), t.start ? this.formatDate(new Date(t.start)) : '');
						this.updateElement(taskRow.querySelector(`#duration_${t.id}`), t.duration ? this.millisecondsToTime(t.duration, true) : '');
						this.updateElement(taskRow.querySelector(`#enqueued_${t.id}`), t.queued ? 'true' : '');
						this.updateElement(taskRow.querySelector(`#fetched_${t.id}`), t.fetched ? 'true' : '');
						if (t.state !== 'new' && t.state !== 'queueud' && t.state !== 'dequeueud') {
							taskRow.querySelector(`#trash_${t.id}`).innerHTML = '';
						}
					}
				} else {
					// add new row
					if (t.state !== 'deleted' && !(t.isRecurring && t.state === 'new')) {
						var row = tasklist.querySelector('tbody').insertRow(0);
						row.id = `task_${t.id}`;
						row.setAttribute('data-task-id', t.id);
						row.classList.add('broadcast-table-row');

						this.addCell(row, 0, null, t.id);
						this.addCell(row, 1, null, t.name);
						this.addCell(row, 2, `state_${t.id}`, state);
						//this.addCell(row, 3, null, t.isRecurring);
						this.addCell(row, 3, null, this.millisecondsToTime(t.time));
						this.addCell(row, 4, `server_${t.id}`, t.server);
						this.addCell(row, 5, `start_${t.id}`, t.start ? this.formatDate(new Date(t.start)) : '');
						this.addCell(row, 6, `duration_${t.id}`, t.duration ? this.millisecondsToTime(t.duration, true) : '');
						this.addCell(row, 7, `enqueued_${t.id}`, t.queued ? 'true' : '');
						this.addCell(row, 8, `fetched_${t.id}`, t.fetched ? 'true' : '');
						var trash = this.addCell(row, 9, `trash_${t.id}`, '');
						if (t.state === 'new' || t.state === 'queueud' || t.state === 'dequeueud') {
							trash.innerHTML = `<div class="trash-box"><i class="trash-button" data-task-id="${t.id}"></i></div>`;
						}
					}
				}
			}
		});
		this.updateElement(document.querySelector('#broadcast-enqueued-count'), cnt);
		this.updateElement(document.querySelector('#broadcast-processed-count'), processedCnt);
		this.updateElement(document.querySelector('#broadcast-failed-count'), failedCnt);
	}

	addCell(row, index, id, value) {
		var cell = row.insertCell(index);
		cell.innerText = value;
		if (id !== null) {
			cell.id = id;
		}

		return cell;
	}

	millisecondsToTime(s, short) {

		// Pad to 2 or 3 digits, default is 2
		function pad(n, z) {
			z = z || 2;
			return ('00' + n).slice(-z);
		}

		var ms = s % 1000;
		s = (s - ms) / 1000;
		var secs = s % 60;
		s = (s - secs) / 60;
		var mins = s % 60;
		var hrs = (s - mins) / 60;
		if (short) {
			return `${pad(mins)}:${pad(secs)}.${pad(ms, 3)}`;
		}
		return `${pad(hrs)}:${pad(mins)}:${pad(secs)}.${pad(ms, 3)}`;
	}

	formatDate(date) {
		var d = date.getDate();
		var m = date.getMonth() + 1;
		var y = date.getFullYear();
		var h = date.getHours();
		var mn = date.getMinutes();
		var s = date.getSeconds();
		return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d) + ' ' + (h <= 9 ? '0' + h : h) + ':' + (mn <= 9 ? '0' + mn : mn) + ':' + (s <= 9 ? '0' + s : s);
	}

	updateElement(elem, data) {
		if (elem) {
			elem.innerText = data;
		}
	}

	showDetail(url) {
		fetch(url,
			{
				method: "GET",
				headers: { 'content-type': 'application/json;  charset=utf-8' }
			}
		).then(function(response) {
			if (!response.ok) {
				throw Error(response.statusText);
			}
			return response.json();
		}).then(function(response) {
			var rows = '';
			
			response.groups.forEach(g => {
				if (g.values.length > 0) {
					rows = rows + `<div class="broadcast-storage-type-row"><span></span><h4>${g.title}</h4></div>`;
					g.values.forEach(p => {
						if (p.key === '') {
							rows = rows +
								`<div class="broadcast-storage-type-row-single">
							<div class="broadcast-storage-type-value">${p.value}</div>
						</div>`;
						} else {
							rows = rows +
								`<div class="broadcast-storage-type-row">
							<div class="broadcast-storage-type-key">${p.key}</div>
							<div class="broadcast-storage-type-value">${p.value}</div>
						</div>`;
						}
					});
				}
			});
			
			var overlay = document.querySelector('#broadcast-data-overlay');
			overlay.querySelector('#broadcast-data-title').innerText = response.title;
			overlay.querySelector('#broadcast-data-key').innerText = response.key;
			overlay.querySelector('#broadcast-data-table').innerHTML = rows;

			overlay.style.display = 'block';
		});
	}

	deleteTask(url) {
		fetch(url,
			{
				method: "POST",
				//headers: { 'content-type': 'application/json;  charset=utf-8' }
			}
		).then(function(response) {
			if (!response.ok) {
				throw Error(response.statusText);
			}

			return response.json();
		}).then(function (response) {
			//TODO: remove task
			var tasklist = document.querySelector('#tasklist');
			if (tasklist) {
				var task = tasklist.querySelector(`tr[data-task-id="${response.id}"]`);
				if (task) {
					tasklist.deleteRow(task.rowIndex);
				}
			}
			// delete the recurring item if it is arecurring task
			var recurringlist = document.querySelector('#recurringlist');
			if (recurringlist) {
				var recurring = recurringlist.querySelector(`tr[data-recurring-reference-id="${response.id}"]`);
				if (recurring) {
					recurringlist.deleteRow(recurring.rowIndex);
				}
			}
			
		});
	}
}

if (dashboardConfig === undefined) {
	dashboardConfig = {
		//TODO: /broadcast/ has to be able to be appended as configuration
		pollUrl: "/broadcast/dashboard/metrics",
		//TODO: /broadcast/ has to be able to be appended as configuration
		dashboardUrl: "/broadcast/dashboard",
		pollInterval: 2000
	};
}

const dashboard = new BroadcastDashboard(dashboardConfig);