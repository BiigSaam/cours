extends Area2D

class_name RockHeadTrigger

var parent:Node2D = null
signal on_parent_collision

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_body_entered(body):
	if parent == body:
#		set_process(false)
		on_parent_collision.emit()
	
