"use strict";

import React from 'react';
import { render } from 'react-dom';
import DayPicker from 'react-day-picker/DayPickerInput';
import moment from 'moment';
import 'moment/locale/ru';
import MomentLocaleUtils from 'react-day-picker/moment';
import 'react-day-picker/lib/style.css';
import '~/jsx/DatePicker/DatePicker.css';

export default class DatePicker extends React.Component {

	constructor(arg) {
		super(arg);
		const {
			id,
			label,
			value,
			divclass,
			labelclass,
			pickerclass,
			onChanged,
			placeholder,
			clearButton,
			dateFormat,
			todayButton
		} = arg;
		this.id = id;
		this.label = label;
		this.placeholder = placeholder != null ? placeholder : "Выберите дату";
		this.clearButton = clearButton == null ? true : clearButton;

		this.todayButton = todayButton != null ? todayButton : "Сегодня";

		this.dateFormat = dateFormat != null ? dateFormat : moment().localeData().longDateFormat("L");

		this.onChanged = onChanged;

		this.divclass = divclass != null ? (divclass == "" ? null : divclass) : "reactDatePickerDivClass";
		this.labelclass = labelclass != null ? (labelclass == "" ? null : labelclass) : "reactDatePickerLabelClass";
		this.pickerclass = pickerclass != null
			? (pickerclass == "" ? null : pickerclass)
			: "reactDatePickerAreaClass";

//	        this.values = value ? moment(value).format(this.dateFormat) : "";
		this.values = value ? value : "";
		this.state = { values: this.values };

	}

	valueChanged(value) {

		var date = "";
		if (value) {
			value = moment(value);
			value.set({ hour: 0, minute: 0, second: 0, millisecond: 0 });
			date = value.format();
		}
		this.setState({ value: date });
		if (typeof this.onChanged == "function") this.onChanged(this, date);
	}

	render() {
		var momentLocaleUtils = MomentLocaleUtils;
		const modifiers = {
			weekends: { daysOfWeek: [0, 6] }
		}

		if (typeof this.onChanged == "function")
			this.state.values = this.props.value != null ? this.props.value : "";
		return(
			<div className={this.divclass}>
				<input type="hidden" id={this.id} name={this.id} value={this.state.values}/>
				<div className={this.labelclass}>
					<label>{this.label}</label>
				</div>
				<div className={this.pickerclass}>
					<DayPicker
						formatDate={ momentLocaleUtils.formatDate }
						parseDate={ momentLocaleUtils.parseDate }
						format="L"
						dayPickerProps={ {
				                        locale: "ru",
				                        localeUtils: MomentLocaleUtils,
				                        todayButton: this.todayButton,
				                        modifiers: modifiers
			                        } }
						placeholder={ this.placeholder }
						value={ this.state.values ? moment(this.state.values).format(this.dateFormat) : "" }
						onChange={this.valueChanged.bind(this)}
						onDayChange={this.valueChanged.bind(this)}/>
				</div>
			</div>
		);
	}

}