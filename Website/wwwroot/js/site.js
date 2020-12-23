let config = {
    position: '',
    draggable: true,
    dropOffBoard: constants.methodResponses.snapback, // this is the default
    onDragStart: events.onDragStart,
    onDrop: events.onDrop
}

document.addEventListener('DOMContentLoaded', (event) => events.init());
