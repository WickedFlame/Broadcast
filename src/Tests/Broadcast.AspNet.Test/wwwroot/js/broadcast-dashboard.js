
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
							reject(error);
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
		//data.forEach(function (pipeline) {
		//	gaucho.updateMetrics(pipeline.serverName, pipeline.pipelineId, pipeline.metrics);
		//	gaucho.updateElements(pipeline.serverName, pipeline.pipelineId, pipeline.elements);
		//});
		this.updateElement(document.querySelector('#broadcast-servers-count'), data.monitor.servers.length);
		this.updateElement(document.querySelector('#broadcast-recurring-count'), data.monitor.recurringTasks.length);

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
		});
		this.updateElement(document.querySelector('#broadcast-enqueued-count'), cnt);
		this.updateElement(document.querySelector('#broadcast-processed-count'), processedCnt);
		this.updateElement(document.querySelector('#broadcast-failed-count'), failedCnt);
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