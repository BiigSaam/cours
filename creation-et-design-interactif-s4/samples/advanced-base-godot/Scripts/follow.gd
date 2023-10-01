extends Camera2D

@export var follow:Node2D
@export var camera:Camera2D

var screen_width = get_viewport_rect().size.x
var camera_target
var target_distance = 20 # You can use screen_width if you want 1/x of the screen etc.
var camera_speed = 60



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if follow.scale.x == 1 :
		camera_target = follow.position.x + target_distance - screen_width/2
		offset.x = min(offset.x + camera_speed, camera_target)
	else:
		camera_target = follow.position.x - target_distance - screen_width/2
		offset.x = max(offset.x - camera_speed, camera_target)
		
#	offset.y = follow.position.y
