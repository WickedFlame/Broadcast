
export class BroadcastBase {
	constructor() {
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
							if (func(response)) {
								// end the polling if the function returns true
								return false;
							}
							resolve(false);
						}).catch(function (error) {
							console.log(error);
							reject(error);
						});
					};
					return new Promise(fn);
				},
			config.pollInterval)
			.then(function () {
				// Polling done, now do something else!
			}).catch(function () {
				// Polling timed out, handle the error!
			});
	}
}