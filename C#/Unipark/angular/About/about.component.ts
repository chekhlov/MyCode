import {Component} from '@angular/core';
import {fadeInOut} from '@services/animations';
import {IleuBaseComponent} from "@components/shared/ileu.base.component";
import {IleuException} from "@helpers/exception";
import {Magnetic} from './magnetic';
import {AppTranslationService} from "@services/app-translation.service";
import {ConfigurationService} from "@services/configuration.service";

@Component({
	selector: 'about'
	, templateUrl: './about.component.html'
	, styleUrls: ['./about.component.scss']
	, animations: [fadeInOut]
})
export class AboutComponent extends IleuBaseComponent {

	teamText: string[] = null;

	magnetic : Magnetic = null;
	languageChangedSubscription: any;

	y : number = 0;

	constructor(public translationService: AppTranslationService, public configuration: ConfigurationService) {
		super();
	}

	ngOnInit() {
		this.languageChangedSubscription = this.translationService.languageChanged$.subscribe(lang => this.updateLanguage(lang));

		this.updateLanguage(this.configuration.language);
		this.magnetic = new Magnetic();
		this.magnetic.init();

		setInterval(this.onTick, 1E3 / 30);
		this.y = 0;
	}

	ngOnDestroy() {
		this.languageChangedSubscription.unsubscribe();
	}

	updateLanguage = (lang : string) => {
		let loadText: string;
		switch (lang) {
			case "en": loadText = require("raw-loader!../about/devteam.en.txt"); break;
			case "ru": loadText = require("raw-loader!../about/devteam.ru.txt"); break;
			case "kk": loadText = require("raw-loader!../about/devteam.kz.txt"); break;
			default: loadText = require("raw-loader!../about/devteam.en.txt");
		}

		this.teamText = loadText.split('\r');
		let div = document.getElementById("about-text");
		let text = '';
		this.teamText.forEach(r => {
			text += this.parseMarkdownToken(r);
		});

		text += `<div class="space"></div>`;
		div.innerHTML = text;
	};

	parseMarkdownToken(text): string {

		text = text.replace('\n', '');
		let _class = '';

		if (text.startsWith('##'))
			text = `<h2>${text.substring(2)}</h2>`;
		else if (text.startsWith('#'))
			text = `<h1>${text.substring(1)}</h1>`;
		else
			text = `<p>${text}</p>`;

		return text;
	}

	onTick = () => {
		this.magnetic.tick();
	}
}
