import React from 'react';
import { render } from 'react-dom';
import _ from 'lodash';
import { Collapse } from 'react-bootstrap';
import { DropDownList, MultiplySelect } from '~/jsx/DropDownList/DropDownList.jsx';
import DatePicker from '~/jsx/DatePicker/DatePicker.jsx';
import './report.css';

export class ReportFilter extends React.Component {
	constructor(arg) {
		super(arg);

		this.defaultFilter = document.reportFilter;

		this.state = {
			filter: _.cloneDeep(this.defaultFilter),
			subscribeStatusVisible: this.defaultFilter.assortmentType == 0 &&
				this.defaultFilter.eventId != 0 &&
				this.defaultFilter.promotionIdList != null &&
				this.defaultFilter.promotionIdList != "",
			actionVisible: this.defaultFilter.assortmentType == 0
		};
	}

	clearFilter() {

		var clearFilter = {
			regionIdList: '',
			memberCategoriesIdList: '',
			clientIdList: '',
			assortmentType: 0,
			eventId: '',
			promotionIdList: '',
			subscribeEventStatusId: 1,
			catalogIdList: '',
			addressIdList: '',
			supplierIdList: '',
			producerIdList: '',
			vitallyImportant: 0,
			isPku: 0,
			dateBegin: this.defaultFilter.dateBegin,
			dateEnd: this.defaultFilter.dateEnd
		}

		this.setState({
			filter: clearFilter,
			subscribeStatusVisible: false,
			actionVisible: true
		});
	}

	BindMultiplySelect(element, arg) {
		const { id, label, url, value, divclass, labelclass, dropdownclass, onChanged, items, placeholder } = arg;
		render(
			<MultiplySelect id={id} label={label} url={url} value={value} divclass={divclass} labelclass={labelclass}
			                dropdownclass={dropdownclass} onChanged={onChanged} items={items} placeholder={placeholder}/>,
			element);
	}

	BindDropDownList(element, arg) {
		const { id, label, url, value, divclass, labelclass, dropdownclass, onChanged } = arg;
		render(<DropDownList id={id} label={label} url={url} value={value} divclass={divclass} labelclass={labelclass}
		                     dropdownclass={dropdownclass} onChanged={onChanged} items={items} placeholder={placeholder}/>,
			element);
	}

	render() {
		return(<div className="reactReport">
			       <MultiplySelect id="RegionIdList" label="Регионы:" value={this.state.filter.regionIdList}
			                       placeholder="Все" url="GetListRegion"
			                       onChanged={(el, value) => {
				                       if (this.state.filter.regionIdList == value) return;
				                       var filter = this.state.filter;
				                       filter.regionIdList = value;
				                       this.setState({ filter: filter });
			                       }}/>
			       <MultiplySelect id="MemberCategoriesIdList" label="Статус клиента:" value={this.state.filter
				       .memberCategoriesIdList}
			                       url="GetListMemberCategories"
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.memberCategoriesIdList = value;
				                       this.setState({ filter: filter });
			                       }}
			                       placeholder="Все"/>
			       <DropDownList id="AssortmentType" label="Тип ассортимента:" value={ this.state.filter.assortmentType }
			                     items={ [
				                     { Id: 0, Name: "Акционный" },
				                     { Id: 1, Name: "Обычный" }
			                     ] }
			                     onChanged={(el, value) => {
				                     var filter = this.state.filter;
				                     filter.assortmentType = value;
				                     filter.eventId = "";
				                     filter.promotionIdList = "";
				                     filter.catalogIdList = "";
				                     filter.producerIdList = "";
				                     filter.clientIdList = "";
				                     filter.addressIdList = "";
				                     var actionVisible = value === 0;
				                     this.setState({
					                     filter: filter,
					                     actionVisible: actionVisible,
					                     subscribeStatusVisible: false
				                     });
			                     }}/>
			       <Collapse in={ this.state.actionVisible} className="reactCollapseAddition">
				       <div>
					       <DropDownList id="EventId" label="Мероприятие:" value={ this.state.filter.eventId }
					                     url="GetListEvents"
					                     clearButton={ true }
					                     filter={ {
						                     filter: this.state.filter,
						                     filterFields: 'memberCategoriesIdList, regionIdList, clientIdList, assortmentType'
					                     }}
					                     onChanged={(el, value) => {
						                     var filter = this.state.filter;
					                       filter.producerIdList = "";
						                     filter.promotionIdList = "",
						                     filter.catalogIdList = "";
						                     filter.eventId = value;
						                     filter.clientIdList = "";
						                     filter.addressIdList = "";
						                     this.setState({
							                     filter: filter,
							                     subscribeStatusVisible: false
						                     });
						                     this.state.subscribeStatusVisible = false;
					                     }}
					                     placeholder="Выберите мероприятие"/>
					       <MultiplySelect id="PromotionIdList" label="Акции:" value={ this.state.filter.promotionIdList }
					                       url="GetListPromotions"
					                       filter={ {
						                       filter: this.state.filter,
						                       filterFields: 'eventId, assortmentType'
					                       }}
					                       onChanged={(el, value) => {
						                       var filter = this.state.filter;
						                       filter.promotionIdList = value;
							                     filter.clientIdList = "";
						                       filter.addressIdList = "";
						                       filter.catalogIdList = "";
						                       filter.producerIdList = "";
				                           filter.supplierIdList = value != null && value != "" ? "0" : "";

						                       var subscribeStatusVisible = filter.eventId != null &&
							                       filter.promotionIdList != null &&
							                       filter.promotionIdList != "";
						                       this.setState({
							                       filter: filter,
							                       subscribeStatusVisible: subscribeStatusVisible
						                       });
						                       this.state.subscribeStatusVisible = subscribeStatusVisible;

					                       }}
					                       placeholder="Все"/>
					       <Collapse in={ this.state.subscribeStatusVisible } className="reactCollapseAddition">
						       <div>
							       <DropDownList id="SubscribeEventStatusId" label="Подписка на акцию:" value={ this.state.filter
								       .subscribeEventStatusId }
							                     items={ [
								                     { Id: 0, Name: "Все" },
								                     { Id: 1, Name: "Подписанные" },
								                     { Id: 2, Name: "Подписанные Производителем и Ассоциацией" },
								                     { Id: 3, Name: "Подписанные Производителем, Ассоциацией и Аптекой" },
								                     { Id: 4, Name: "Неподписанные" }
							                     ] }
							                     onChanged={(el, value) => {
								                     var filter = this.state.filter;
								                     filter.subscribeEventStatusId = value;
								                     this.setState({ filter: filter });
							                     }}/>
						       </div>
					       </Collapse>
				       </div>
			       </Collapse>
			       <MultiplySelect id="CatalogIdList" label="Товары:" value={ this.state.filter.catalogIdList }
			                       dynamic
			                       url="GetListProducts"
			                       filter={ {
				                       filter: this.state.filter,
				                       filterFields: 'assortmentType, eventId, promotionIdList'
			                       }}
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.catalogIdList = value;
				                       this.setState({ filter: filter });
			                       }}
			                       placeholder="Все (введите товара наименование для поиска)"/>
			       <MultiplySelect id="ProducerIdList" label="Производители:" value={ this.state.filter.producerIdList }
			                       dynamic
			                       url="GetListProducers"
			                       filter={ {
				                       filter: this.state.filter,
				                       filterFields: 'assortmentType, eventId, promotionIdList'
			                       }}
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.producerIdList = value;
				                       this.setState({ filter: filter });
			                       }}
			                       placeholder="Все"/>
			       <MultiplySelect id="ClientIdList" label="Участники:" value={ this.state.filter.clientIdList }
			                       url="GetListClients"
			                       filter={ {
				                       filter: this.state.filter,
				                       filterFields: 'memberCategoriesIdList, regionIdList, assortmentType, eventId, promotionIdList, subscribeEventStatusId'
			                       }}
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.clientIdList = value;
				                       this.setState({
					                       filter: filter
				                       });
			                       }}
			                       placeholder="Все"/>
			       <MultiplySelect id="AddressIdList" label="Адреса аптек:" value={ this.state.filter.addressIdList }
			                       url="GetListAddresses"
			                       filter={ {
				                       filter: this.state.filter,
				                       filterFields: 'memberCategoriesIdList, regionIdList, assortmentType, clientIdList, eventId, promotionIdList, subscribeEventStatusId'
			                       }}
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.addressIdList = value;
				                       this.setState({ filter: filter });
			                       }}
			                       placeholder="Все"/>
			       <MultiplySelect id="SupplierIdList" label="Поставщики:" value={ this.state.filter.supplierIdList }
			                       url="GetListSuppliers"
			                       filter={ {
				                       filter: this.state.filter,
				                       filterFields: 'memberCategoriesIdList, regionIdList, clientIdList, promotionIdList, eventId'
			                       }}
			                       onChanged={(el, value) => {
				                       var filter = this.state.filter;
				                       filter.supplierIdList = value;
				                       this.setState({ filter: filter });
			                       }}
			                       placeholder="Все"/>
			       <DropDownList id="VitallyImportant" label="ЖНВЛП:" value={ this.state.filter.vitallyImportant }
			                     items={ [
				                     { Id: 0, Name: "Не важно" },
				                     { Id: 1, Name: "Да" },
				                     { Id: 2, Name: "Нет" }
			                     ] }
			                     onChanged={(el, value) => {
				                     var filter = this.state.filter;
				                     filter.vitallyImportant = value;
				                     this.setState({ filter: filter });
			                     }}/>
			       <DropDownList id="IsPku" label="ПКУ:" value={ this.state.filter.isPku }
			                     items={ [
				                     { Id: 0, Name: "Не важно" },
				                     { Id: 1, Name: "Да" },
				                     { Id: 2, Name: "Нет" }
			                     ] }
			                     onChanged={(el, value) => {
				                     var filter = this.state.filter;
				                     filter.isPku = value;
				                     this.setState({ filter: filter });
			                     }}/>
			       <div className="reactReportDatePeriod">
				       <div className="reactReportDatePeriodLabel">
					       <label>Период:</label>
				       </div>
				       <div className="reactReportDatePeriodArea">
					       <div className="reactReportDate">
						       <DatePicker id="DateBegin" label="c:"
						                   value={ this.state.filter.dateBegin }
						                   onChanged={(el, value) => {
							                   var filter = this.state.filter;
							                   filter.dateBegin = value;
							                   this.setState({ filter: filter });
						                   }}/>
					       </div>
					       <div className="reactReportDate">
						       <DatePicker id="DateEnd" label="по:"
						                   value={ this.state.filter.dateEnd }
						                   onChanged={(el, value) => {
							                   var filter = this.state.filter;
							                   filter.dateEnd = value;
							                   this.setState({ filter: filter });
						                   }}/>
					       </div>
				       </div>
			       </div>
			       <div className="reactReportButtonsDiv">
				       <button type="submit" className="btn btn-primary" value="Поиск">
					       Поиск
				       </button>
				       <label>|</label>
				       <button type="submit" name="exportToExcel" value="true" className="btn btn-success">Экспорт в Excel</button>
				       <label>|</label>
				       <a className="btn btn-default" onClick={this.clearFilter.bind(this)}>Очистить</a>
			       </div>
		       </div>
		);
	}
};

var fn = function(fnBind, el) {
	const val = $(el);
	const id = val.attr('id');
	// преобразуем адрес вызова по адресу контроллера
	const
		url = val.attr('url'); //window.location.href.substring(0, window.location.href.lastIndexOf('/Members') + 8) + "/"+ val.attr('url') + "/";

	if (id !== null) {

		var items = [];

		// Получаем вложенные Items
		val.children('item').each(function() {
			var chld = $(this);
			items.push({ Id: chld.attr('key'), Name: chld.text() });
		});

		var arg = {
			id: id,
			label: val.attr('label'),
			url: url,
			value: val.attr('value'),
			placeholder: val.attr('placeholder'),
			divclass: val.attr('divclass'),
			labelclass: val.attr('labelclass'),
			dropdownclass: val.attr('dropdownclass'),
			onChanged: val.attr('onChanged'),
			items: items
		};

		// заменяем div c параметрами на сгенерированный код
		var element = document.getElementById(id);
		var parentNode = element.parentNode;
		var div = document.createElement('div');
		div.setAttribute('id', 'reactMultiplySelect' + id);
		div.setAttribute('class', val.attr('class'));
		parentNode.replaceChild(div, element);

		fnBind(div, arg);
	};
}

var filter = new ReportFilter();

var el = $('div.reactMultiplySelect');
if (el !== null) {
	el.each(function() { fn(filter.BindMultiplySelect, this); });
}

el = $('div.reactDropDown');
if (el !== null) {
	el.each(function() { fn(filter.BindDropDownList, this); });
}

render(<ReportFilter/>, document.getElementById("reactFilterRoot"));