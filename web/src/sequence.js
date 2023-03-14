const paperWidth = 800;
const paperHeight = 600;

let sd = joint.shapes.sd
const graph = new joint.dia.Graph();
const paper = new joint.dia.Paper({
    el: document.getElementById('paper'),
    model: graph,
    width: paperWidth,
    height: paperHeight,
    gridSize: 1,
    frozen: true,
    interactive: function (elementView) {
        if (elementView.model.get('type') == 'sd.Details') {
            return true
        } else {
            return false
        }
    }
})


let roles = new Array();
let lifelines = new Array();

let roleShift = 300
let messageShift = 40

function addDestination(name) {
    const role = new sd.Role({position: {x: roles.length * roleShift}});
    role.setName(name);
    role.addTo(graph);

    const lifeline = new sd.Lifeline();
    lifeline.attachToRole(role, paperHeight);
    lifeline.addTo(graph);

    roles.push(role)
    lifelines.push(lifeline)
}

/*
    from, to - id of the destinations
 */
function addMessage(id, text, from, to) {
    const message = new sd.Message();

    message.setFromTo(lifelines[from], lifelines[to]);
    message.setStart(30 + id * messageShift);
    message.setDescription(text);
    message.addTo(graph);
    message.mid = id;
}

// mouseenter/mouseleave

paper.on('link:pointerdown', function (linkView) {
    switch (linkView.model.get('type')) {
        case 'sd.Message': {
            var model = linkView.model;
            if (model.details == null) {
                let box = model.getBBox()
                var details = new sd.Details({position: {x: box.x, y: 30}})
                let text = obj['messages'][model.mid]['text']
                details.setText(text)
                details.resize(500, text.split('\n').length * 20);
                details.message = model.id
                details.addTo(graph)
                model.details = details
            }
        }
    }
});

paper.on('details:button:pointerdown', function (elementView, evt) {
    elementView.model.remove();
});

let json = sample()
const obj = JSON.parse(json);

function fill(data) {
    data['roles'].forEach(role => addDestination(role))
    data['messages'].forEach((el, i) =>
        addMessage(i, el.label, el.from, el.to)
    )
}

fill(obj)

paper.unfreeze();



