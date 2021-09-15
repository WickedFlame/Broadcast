
export class BroadcastDashboard {
	constructor(config) {
		setTimeout(() => {
			this.startPolling(config, (data) => this.updateDashboard(data, this));
		}, 3000);
	}

	poll(fn, interval) {
		interval = interval || 1000;

		var checkCondition = function (resolve, reject) {
			fn().then(function (result) {
				setTimeout(checkCondition, interval, resolve, reject);
			});
		};

		return new Promise(checkCondition);
	}

	startPolling(config, func) {
		this.poll(function () {
					var fn = function (resolve, reject) {
						var url = config.pollUrl;
						fetch(url,
							{
								method: "GET",
								headers: { 'content-type': 'application/json;  charset=utf-8' }
							}
						).then(function (response) {
							if (!response.ok) {
								throw Error(response.statusText);
							}
							return response.json();
						}).then(function (response) {
							func(response);
							resolve(false);
						}).catch(function (error) {
							console.log(error);
							//reject(error);
							resolve(false);
						});
					};
					return new Promise(fn);
				},
				1000)
			.then(function () {
				// Polling done, now do something else!
			}).catch(function () {
				// Polling timed out, handle the error!
			});
	}

	updateDashboard(data, dashboard) {
		this.updateElement(document.querySelector('#broadcast-servers-count'), data.monitor.servers.length);
		this.updateElement(document.querySelector('#broadcast-recurring-count'), data.monitor.recurringTasks.length);

		var recurringlist = document.querySelector('#recurringlist');
		if (recurringlist) {
			data.monitor.recurringTasks.forEach(t => {
				var name = t.name.replace('.', '_');
				var taskRow = recurringlist.querySelector(`#recurring_${name}`);

				if (taskRow) {
					taskRow.querySelector(`#referenceid_${name}`).innerText = t.referenceId;
					taskRow.querySelector(`#nextexecution_${name}`).innerText = this.formatDate(new Date(t.nextExecution));
				} else {
					// add new row
					var row = recurringlist.querySelector('tbody').insertRow(0);
					row.id = `recurring_${name}`;

					this.addCell(row, 0, null, t.name);
					this.addCell(row, 1, `referenceid_${name}`, t.referenceId);
					this.addCell(row, 2, null, this.formatDate(new Date(t.nextExecution)));
					this.addCell(row, 3, null, this.millisecondsToTime(t.interval));
				}
			});
		}


		var tasklist = document.querySelector('#tasklist');

		var cnt = 0;
		var processedCnt = 0;
		var failedCnt = 0;
		data.monitor.tasks.forEach(t => {
			if (t.state !== 4 && t.state !== 5) {
				cnt = cnt + 1;
			}else if (t.state === 4) {
				processedCnt = processedCnt + 1;
			} else if (t.state === 5) {
				failedCnt = failedCnt + 1;
			}

			if (tasklist) {
				var taskRow = document.querySelector(`#task_${t.id}`);
				var state = t.state === 0
					? 'New'
					: t.state === 1
					? 'Queued'
					: t.state === 2
					? 'Dequeued'
					: t.state === 3
					? 'InProcess'
					: t.state === 4
					? 'Processed'
					: t.state === 5
					? 'Faulted'
					: 'Unknown';

				if (taskRow) {
					taskRow.querySelector(`#state_${t.id}`).innerText = state;
					taskRow.querySelector(`#server_${t.id}`).innerText = t.server;
					taskRow.querySelector(`#start_${t.id}`).innerText = t.start ? this.formatDate(new Date(t.start)) : '';
					taskRow.querySelector(`#duration_${t.id}`).innerText = t.duration;
				} else {
					// add new row
					if (!tasklist.classList.contains('processed') || t.state === 4) {
						var row = tasklist.querySelector('tbody').insertRow(0);
						row.id = `task_${t.id}`;

						this.addCell(row, 0, null, t.id);
						this.addCell(row, 1, null, t.name);
						this.addCell(row, 2, `state_${t.id}`, state);
						this.addCell(row, 3, null, t.isRecurring);
						this.addCell(row, 4, null, this.millisecondsToTime(t.time));
						this.addCell(row, 5, `server_${t.id}`, t.server);
						this.addCell(row, 6, `start_${t.id}`, t.start ? this.formatDate(new Date(t.start)) : '');
						this.addCell(row, 7, `duration_${t.id}`, t.duration);
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
	}

	millisecondsToTime(s) {

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
		elem.innerText = data;
	}
}

if (dashboardConfig === undefined) {
	dashboardConfig = {
		pollUrl: "/dashboard/metrics",
		pollInterval: 2000
	};
}

const dashboard = new BroadcastDashboard(dashboardConfig);