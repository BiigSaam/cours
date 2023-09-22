extends Area2D

@export var speed = 500;

var direction: Vector2 = Vector2.LEFT;

func _process(delta):
	position += direction * speed * delta
	
