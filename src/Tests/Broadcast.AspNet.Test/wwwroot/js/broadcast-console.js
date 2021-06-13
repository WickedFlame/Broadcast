﻿import { BroadcastBase } from './broadcast-base.js';

export class BroadcastConsole extends BroadcastBase {
	constructor(config) {
		super();

		this.renderConsole(config);
		setTimeout(() => {
			this.startPolling(config, (data) => this.updateConsole(data, this));
		}, 3000);
	}

	

	updateConsole(data, console) {
		var consElem = document.querySelector('#broadcast-console');
		if (!consElem) {
			return;
		}

		this.updateElement(consElem.querySelector('#broadcast-servers-count'), data.monitor.servers.length);
		this.updateElement(consElem.querySelector('#broadcast-recurring-count'), data.monitor.recurringTasks.length);

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
		this.updateElement(consElem.querySelector('#broadcast-enqueued-count'), cnt);
		this.updateElement(consElem.querySelector('#broadcast-processed-count'), processedCnt);
		this.updateElement(consElem.querySelector('#broadcast-failed-count'), failedCnt);
	}

	updateElement(elem, data) {
		elem.innerText = data;
	}

	renderConsole(config) {
		var console = `<div class="broadcast-console" id="broadcast-console">
	<div class="broadcast-console-panel">
		<div class="broadcast-console-title">Broadcast</div>
		<div class="broadcast-metric"><span>Servers</span><span id="broadcast-servers-count">-</span></div>
		<div class="broadcast-metric"><span>Recurring Tasks</span><span id="broadcast-recurring-count">-</span></div>
		<div class="broadcast-metric"><span>Enqueued Tasks</span><span id="broadcast-enqueued-count">-</span></div>
		<div class="broadcast-metric"><span>Processed Tasks</span><span id="broadcast-processed-count">-</span></div>
		<div class="broadcast-metric"><span>Failed Tasks</span><span id="broadcast-failed-count">-</span></div>
	</div>	
</div>`;

		// add the console
		let div = document.createElement('div');
		div.innerHTML = console.trim();
		document.querySelector('body').appendChild(div.firstChild);
	}
}

if (consoleConfig === undefined) {
	consoleConfig = {
		pollUrl: "/dashboard/metrics",
		pollInterval: 2000
	};
}

const console = new BroadcastConsole(consoleConfig);