@tool

extends RigidBody2D

var next_position:Vector2
@export var current_trigger_index = 0 :
	get:
		return current_trigger_index
	set(value):
		print("value " + str(value))
		current_trigger_index = value
		
var speed:int = 750
@export var list_triggers:Array[RockHeadTrigger]
var dir

func _ready():
	init_triggers()
	enable_triggers()
	get_next_direction(list_triggers[current_trigger_index].position)

	lock_rotation = true
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
#	if Input.is_action_just_pressed("ui_accept"):
#		change_direction()

func init_triggers():
	var trigger
	for index in list_triggers.size():
		trigger = list_triggers[index]
		trigger.parent = self
		
func enable_triggers():
	for index in list_triggers.size():
		var is_current_trigger = (index == current_trigger_index)

		list_triggers[index].set_collision_mask_value(2, !is_current_trigger)
		list_triggers[index].set_collision_mask_value(1, is_current_trigger)
	
func get_next_direction(next_trigger_pos):
	await get_tree().create_timer(1.5).timeout
	next_position = (next_trigger_pos - position)
	next_position = next_position.normalized()
	
	var copy = next_position.abs()
	if copy.x > copy.y:
		next_position.y = 0
	else:
		next_position.x = 0
#	print(linear_velocity)
#	apply_impulse(next_position * 750)
#Parser Error: Function "add_central_force()" not found in base self. Did you mean to use "apply_central_force()"?
#	constant_force = Vector2.ZERO
#	add_constant_force(next_position * speed)
#
##	print(next_position * (speed * 40))
#	set_axis_velocity(next_position * 200)
	
#	return next_position
	linear_velocity = Vector2(next_position.x * speed, next_position.y * speed)
	print(linear_velocity)
#	add_constant_force(next_position * (speed * 40))

func _on_rock_head_trigger_on_parent_collision():
	pass
	change_direction()
	
func change_direction():
	current_trigger_index = (current_trigger_index + 1) % list_triggers.size()
	enable_triggers()
	get_next_direction(list_triggers[current_trigger_index].position)


func _on_body_entered(body):
	print("_on_body_entered " + body.name)
	pass # Replace with function body.
