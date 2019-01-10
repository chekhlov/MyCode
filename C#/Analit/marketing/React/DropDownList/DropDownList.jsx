"use strict";

import React from 'react';
import { render } from 'react-dom';
import Select from 'react-select'
import _ from 'lodash';
import debounce from 'lodash/debounce';
import axios from 'axios';
import 'react-select/dist/react-select.css';
import './DropDownList.css';

export class DropDownList extends React.Component {

	constructor(arg) {
		super(arg);
		const {
			id,
			label,
			url,
			value,
			divclass,
			labelclass,
			dropdownclass,
			onChanged,
			items,
			placeholder,
			filter,
			idKey,
			valueKey,
			clearButton,
			dynamic
		} = arg;
		this.id = id;
		this.label = label;
		this.url = url;
		this.placeholder = placeholder != null ? placeholder : "";
		this.idKey = idKey != null ? idKey : "Id";
		this.valueKey = valueKey != null ? valueKey : "Name";
		this.dynamic = dynamic != null ? dynamic : false;

		this.multi = false;
		this.clearButton = clearButton == null ? false : clearButton;

		this.items = Array.isArray(items) ? items : [];
		this.filter = filter;
		this.prevFilter = null;

		this.onChanged = onChanged;

		this.divclass = divclass != null ? (divclass == "" ? null : divclass) : "reactDropDownDivClass";
		this.labelclass = labelclass != null ? (labelclass == "" ? null : labelclass) : "reactDropDownLabelClass";
		this.dropdownclass = dropdownclass != null ? (dropdownclass == "" ? null : dropdownclass) : "reactDropDownAreaClass";

		this.value = value != null ? value : "";
		this.state = { values: this.value, items: this.items, isLoading: false };
	}

	// Вызывается React после того как компонент создан (загружаем данные)
	componentDidMount = () => {
		this.loadData();
	};

	inputChange = (input) => {
		// if (input != "")
			this.loadData(input);
	}

	loadData = (input) => {
		if (this.url == null) return;

		this.setState({ isLoading: true });
		// клонируем фильтр - добавление поля влияет на другие связанные компоненты.
		var filter = this.props.filter ? _.clone(this.props.filter.filter) : {};
		filter.value = input;

		axios.post(this.url, filter, { timeout: 35000})
			.then(result  => {
					this.setState({
						items: result.data,
						isLoading: false });
				},
				() => {
					this.setState({
						isLoading: false
					});
				});
		this.prevFilter = filter;
	}

	valueChanged = (value) =>  {
		this.setState({ values: value });
		if (typeof this.onChanged == "function") this.onChanged(this, value);
		if (this.dynamic) this.loadData();
	}

	// Создает новый объект из filter только с указанными полями в filterFields
	prepareFilter = (filter, filterFields) => {

		var newfilter = null;

		if (!filter) return newfilter;

		var splitFields = filterFields.split(",").map(i => i.trim());

		splitFields.forEach(function(el) {
			if (filter.hasOwnProperty(el)) {
				if (newfilter == null) newfilter = {};
				newfilter[el] = filter[el];
			}
		});
		return newfilter;
	}

	// Вызывается React для проверки необходимости обновления компонента
	// возвращает: true - если компонент должен обновится
	// false - обновление не требуется
	shouldComponentUpdate = (nextProps, nextState) => {

		// мы реагируем только на изменение value, items и необходимых свойст в filter
		var update = nextProps.value != nextState.value;

		// используем lodash.isEqual для сравнения объектов
		update = update | !_.isEqual(nextProps.items, nextState.items);

		if (this.props.filter != null) {
			var filter = this.prepareFilter(this.props.filter.filter, this.props.filter.filterFields);
			var prevFilter = this.prepareFilter(this.prevFilter, this.props.filter.filterFields);

			if (!_.isEqual(filter, prevFilter)) {
				this.loadData();
				update = true;
			}
		}

		update = update | nextProps.isLoading != nextState.isLoading;

		return update;
	}

	render = () => {

		// Если указан обработчик onChanged - то обновлением значение value должно происходить обработчике (родительском классе)
		if (typeof this.onChanged == "function") this.state.values = this.props.value != null ? this.props.value : "";

		return(
			<div className={this.divclass}>
				<input type="hidden" id={this.id} name={this.id} value={this.state.values}/>
				<div className={this.labelclass}>
					<label>{this.label}</label>
				</div>
				<Select
					className={this.dropdownclass}
					placeholder={this.placeholder}
					labelKey={this.valueKey}
					valueKey={this.idKey}
					clearable={this.clearButton}
					clearAllText="Очистить"
					clearValueText="Очистить"
					noResultsText="Нет данных"
					simpleValue

					closeOnSelect={ !this.multi }
					multi={this.multi}
					isLoading = {this.state.isLoading}

					options={this.state.items}
					value={this.state.values}
					onChange={this.valueChanged}
					onInputChange={this.dynamic ? debounce(this.inputChange, 300) : null}
					onClear={this.dynamic ? debounce(this.inputChange, 300) : null}
					onSelectResetsInput={ !this.dynamic }/>
			</div>
		);
	}
}

export class MultiplySelect extends DropDownList {

	constructor(arg) {
		super(arg);
		const { clearButton } = arg;

		this.multi = true;
		this.clearButton = clearButton == null ? true : clearButton;
	}
}