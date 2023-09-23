extends RigidBody2D

var right_raycast:RayCast2D
var current_trigger_index = 0
var next_position:Vector2
var speed:int = 750
@export var list_triggers:Array[RockHeadTrigger]
var dir 
# Called when the node enters the scene tree for the first time.
func _ready():
	init_triggers()
	enable_triggers()
	get_next_direction(list_triggers[0].position)
	
	lock_rotation = true
#	right_raycast = $RayCast2D

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	if Input.is_action_just_pressed("ui_accept"):
#		print(dir)

func init_triggers():
	var trigger
	for index in list_triggers.size():
		trigger = list_triggers[index]
		trigger.parent = self
		
func enable_triggers():
	for index in list_triggers.size():
		var is_current_trigger = index == current_trigger_index
		list_triggers[index].set_collision_mask_value(2, !is_current_trigger)
		list_triggers[index].set_collision_mask_value(1, is_current_trigger)
#		list_triggers[index].set_process(index == current_trigger_index)
		
func _physics_process(delta):
#	dir = await get_next_direction(list_triggers[current_trigger_index].position)
#	set_linear_velocity(dir * (speed * 15))
	pass

func get_next_direction(next_trigger_pos):
	await get_tree().create_timer(1.2).timeout
	var next_position:Vector2 = (next_trigger_pos - position)
	next_position = next_position.normalized()
	
	
	var copy = next_position.abs()
	if copy.x > copy.y:
		next_position.y = 0
	else:
		next_position.x = 0

#	constant_force = Vector2.ZERO
#	print(next_position)

	set_linear_velocity(next_position * speed)
	
	return next_position
#	linear_velocity = Vector2(next_position.x * speed, next_position.y * speed)
#	add_constant_force(next_position * (speed * 40))

func _on_rock_head_trigger_on_parent_collision():
	current_trigger_index = (current_trigger_index + 1) % list_triggers.size()
	enable_triggers()
	get_next_direction(list_triggers[current_trigger_index].position)


func _on_body_entered(body):
	print("_on_body_entered")
	pass # Replace with function body.
