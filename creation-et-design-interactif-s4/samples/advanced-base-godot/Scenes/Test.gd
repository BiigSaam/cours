extends Sprite2D

var pos := Vector2(0, 0)
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with functin body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pos.x += 5 
	position = pos


func _on_area_2d_area_entered(area):
	pass # Replace with function body.
