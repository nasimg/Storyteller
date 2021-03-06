var React = require("react");
var HeaderRow = require('./header-row');
var uuid = require('node-uuid');


function TablePreview({cells, title, section, rows, isStepthrough}){
	var tableWidth = cells.length + 1;

	return (
		<table className="table table-bordered table-hover" id={section.id}>
			<thead>
				<tr><th colSpan={tableWidth}>{title}</th></tr>
				<HeaderRow cells={cells} hideActions={true}/>
			</thead>
			<tbody>
				{rows}
			</tbody>
		</table>

	);
}


module.exports = TablePreview;