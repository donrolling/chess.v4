let config = {
    position: '',
    draggable: true,
    dropOffBoard: constants.snapback, // this is the default
    onDragStart: events.onDragStart,
    onDrop: events.onDrop
}

document.addEventListener('DOMContentLoaded', (event) => events.init());
