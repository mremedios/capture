(function (joint) {

    let standard = joint.shapes.standard;
    let dia = joint.dia;

    standard.Rectangle.define('sd.RoleGroup', {
        z: 1,
        attrs: {
            body: {
                stroke: '#DDDDDD',
                strokeWidth: 1,
                fill: '#F9FBFA'
            },
            label: {
                refY: null,
                refX: null,
                y: 'calc(h+2)',
                x: 'calc(w/2)',
                textAnchor: 'middle',
                textVerticalAnchor: 'top',
                fontSize: 12,
                fontFamily: 'sans-serif',
                textWrap: {
                    width: -10
                }
            }
        }
    }, {
        placeholder: 'What\'s the group\'s name?',

        fitRoles: function () {
            this.fitEmbeds({padding: 10});
        }
    });

    standard.Rectangle.define('sd.Role', {
        z: 2,
        size: {width: 100, height: 80},
        attrs: {
            body: {
                stroke: '#A0A0A0',
                strokeWidth: 1,
                rx: 2,
                ry: 2
            },
            label: {
                fontSize: 12,
                fontFamily: 'sans-serif',
                textWrap: {
                    width: -10
                }
            }
        }
    }, {
        placeholder: 'What\'s the role?',

        setName: function (name) {
            this.attr(['label', 'text'], name);
        }
    });

    standard.Link.define('sd.Lifeline', {
        z: 3,
        attrs: {
            line: {
                stroke: '#A0A0A0',
                strokeWidth: 1,
                strokeDasharray: '5,2',
                targetMarker: null
            }
        }
    }, {
        attachToRole: function (role, maxY) {
            const roleCenter = role.getBBox().center();
            this.set({
                source: {id: role.id},
                target: {x: roleCenter.x, y: maxY}
            });
            role.embed(this);
        }
    });

    dia.Link.define('sd.LifeSpan', {
        z: 4,
        attrs: {
            line: {
                connection: true,
                stroke: '#222222',
                strokeWidth: 2
            },
            wrapper: {
                connection: true
            },
            icon: {
                atConnectionRatioIgnoreGradient: 0.5
            }
        }
    }, {
        markup: [{
            tagName: 'path',
            selector: 'line',
            attributes: {
                'fill': 'none',
                'pointer-events': 'none'
            }
        }, {
            tagName: 'path',
            selector: 'wrapper',
            attributes: {
                'fill': 'none',
                'stroke': 'transparent',
                'stroke-width': 20
            }
        }, {
            tagName: 'g',
            selector: 'icon',
            children: [{
                tagName: 'circle',
                attributes: {
                    'r': 12,
                    'fill': '#222222'
                }
            }, {
                tagName: 'path',
                attributes: {
                    'd': 'M -3 -5 3 -5 3 -2 -3  2 -3 5 3 5 3 2 -3 -2 Z',
                    'stroke': '#FFFFFF',
                    'stroke-width': 1,
                    'fill': 'none'
                }
            }]
        }],
        attachToMessages: function (from, to) {
            this.source(from, {anchor: {name: 'connectionRatio', args: {ratio: 1}}});
            this.target(to, {anchor: {name: 'connectionRatio', args: {ratio: 0}}});
        }
    });

    standard.Link.define('sd.Message', {
        source: {anchor: {name: 'connectionLength'}},
        target: {anchor: {name: 'connectionPerpendicular'}},
        attrs: {
            line: {
                stroke: '#4666E5',
                sourceMarker: {
                    'type': 'path',
                    'd': 'M -3 -3 -3 3 3 3 3 -3 z',
                    'stroke-width': 3
                }
            },
            wrapper: {
                strokeWidth: 20,
                cursor: 'grab'
            }
        }
    }, {
        defaultLabel: {
            markup: [{
                tagName: 'rect',
                selector: 'labelBody'
            }, {
                tagName: 'text',
                selector: 'labelText'
            }, {
                tagName: 'text',
                selector: 'buttonLabel'
            }
            ],
            attrs: {
                labelBody: {
                    ref: 'labelText',
                    width: 'calc(w + 20)',
                    height: 'calc(h + 10)',
                    x: 'calc(x - 10)',
                    y: 'calc(y - 5)',
                    rx: 2,
                    ry: 2,
                    fill: '#4666E5'
                },
                labelText: {
                    fill: '#FFFFFF',
                    fontSize: 12,
                    fontFamily: 'sans-serif',
                    textAnchor: 'middle',
                    textVerticalAnchor: 'middle',
                    cursor: 'pointer'
                },
                buttonLabel: {
                    pointerEvents: 'none',
                    x: 'calc(x)',
                    y: 19,
                    textAnchor: 'middle',
                    textVerticalAnchor: 'middle',
                    fill: 'gray',
                    fontSize: 10,
                }
            }
        },
        setStart: function (y) {
            this.prop(['source', 'anchor', 'args', 'length'], y);
        },
        setFromTo: function (from, to) {
            this.prop({
                source: {id: from.id},
                target: {id: to.id}
            });
        },
        setDescription: function (description, time) {
            this.labels([{
                attrs:
                    {
                        labelText:
                            {text: description},
                        buttonLabel:
                            {text: time}
                    }
            },
            ]);
        }
    });

    dia.Element.define('sd.Details', {
        attrs: {
            body: {
                width: 'calc(w)',
                height: 'calc(h)',
                strokeWidth: 2,
                stroke: 'black',
                fill: 'white'
            },
            label: {
                textVerticalAnchor: 'middle',
                textAnchor: 'left',
                x: 'calc(0.01 * w) ',
                y: 'calc(0.5*h)',
                fontSize: 14,
                fill: 'black'
            },
            button: {
                cursor: 'pointer',
                ref: 'buttonLabel',
                width: 'calc(1.5*w)',
                height: 'calc(1.5*h)',
                x: 'calc(x-calc(0.25*w))',
                y: 'calc(y-calc(0.25*h))',
                event: 'details:button:pointerdown',
                fill: '#FFF123',
                stroke: 'black',
                strokeWidth: 2
            },
            buttonLabel: {
                pointerEvents: 'none',
                x: 'calc(w)',
                y: 0,
                textAnchor: 'middle',
                textVerticalAnchor: 'middle',
                text: ' x ',
                fill: 'black',
                fontSize: 8,
                fontWeight: 'bold'
            }
        }
    }, {
        markup: [{
            tagName: 'rect',
            selector: 'body',
        }, {
            tagName: 'text',
            selector: 'label'
        }, {
            tagName: 'rect',
            selector: 'button'
        }, {
            tagName: 'text',
            selector: 'buttonLabel'
        }
        ],
        setText: function (text) {
            this.attr(['label', 'text'], text);
        }
    });

})(joint);