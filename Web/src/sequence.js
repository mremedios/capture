function drawDiagram(obj, element) {
    let roleShift = 400
    let messageShift = 40
    let roleHeight = obj['messages'].length * messageShift + 150 
    let width = obj['endpoints'].length * roleShift
    let sd = joint.shapes.sd

    const graph = new joint.dia.Graph();
    const paper = new joint.dia.Paper({
        el: element,
        model: graph,
        width: width,
        height: roleHeight,
        async: true,
        frozen: true,
        interactive: function (elementView) {
            return (elementView.model.get('type') === 'sd.Details' ||
                elementView.model.get('type') === 'html.Element')
        },
    })

    // Container for all HTML views inside paper
    let htmlContainer = document.createElement('div');
    htmlContainer.style.pointerEvents = 'none';
    htmlContainer.style.position = 'absolute';
    htmlContainer.style.inset = '0';
    paper.el.appendChild(htmlContainer);
    paper.htmlContainer = htmlContainer;

    let roles = [];
    let lifelines = [];

   

    function addDestination(name) {
        const role = new sd.Role({position: {x: roles.length * roleShift}});
        role.setName(name);
        role.addTo(graph);

        const lifeline = new sd.Lifeline();
        lifeline.attachToRole(role, roleHeight);
        lifeline.addTo(graph);

        roles.push(role)
        lifelines.push(lifeline)
    }

    /*
        from, to - id of the destinations
     */
    function addMessage(id, text, from, to, time) {
        const message = new sd.Message();

        message.setFromTo(lifelines[from], lifelines[to]);
        message.setStart(30 + id * messageShift);
        message.setDescription(text, time);
        message.addTo(graph);
        message.mid = id;
    }
    
    paper.on('link:pointerdown', function (linkView) {
            let model = linkView.model
            switch (model.get('type')) {
                case 'sd.Message': {
                    if (model.opened == null) {
                        let box = linkView.getBBox()
                        createDetailsWindow(model, box.x, box.y + 30, obj['messages'][model.mid], element)
                    }
                }
            }
        }
    );


    paper.on('button:pointerdown', function (elementView, evt) {
        elementView.model.remove();
        elementView.model.messageModel.opened = null
    });


    function fill(data) {
        console.log(data['messages'].length)
        data['endpoints'].forEach(role => addDestination(role))
        data['messages'].forEach((el, i) =>
            addMessage(i, el.label.substring(0, 50), el.from, el.to, el.at)
        )
    }

    fill(obj)

    paper.unfreeze();
}

function htmlToElement(html) {
    var template = document.createElement('template');
    html = html.trim(); // Never return a text node of whitespace as the result
    template.innerHTML = html;
    return template.content.firstChild;
}

function createDetailsWindow(model, x, y, jsonEl, div) {
    let pos = div.getBoundingClientRect()
    model.opened = true
    let el = htmlToElement(
        `<div class="card" style="position: absolute" ">
                <button type="button" class="btn-close" aria-label="Close"></button>
                <div class="card-body"></div>  
              </div>`
    );
    el.style.left = x + pos.x + 'px'
    el.style.top = y + pos.y + 'px'
    el.querySelector('.card-body').textContent = jsonEl['text']
    el.querySelector('.btn-close').addEventListener('click', function () {
        el.remove();
        model.opened = null
    });
    $( function() {
        $(".card").draggable();
    } );
    document.body.appendChild(el);
}   