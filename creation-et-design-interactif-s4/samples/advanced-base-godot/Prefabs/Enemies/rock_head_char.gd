extends CharacterBody2D

var speed:int = 750
@export var current_trigger_index = 0
@export var list_triggers:Array[RockHeadTrigger]

func _ready():
	init_triggers()
	enable_triggers()
	get_next_direction(list_triggers[current_trigger_index].position)

func init_triggers():
	var trigger
	for index in list_triggers.size():
		trigger = list_triggers[index]
		trigger.parent = self
		trigger.on_parent_collision.connect(_on_rock_head_trigger_on_parent_collision)
		
func enable_triggers():
	for index in list_triggers.size():
		var is_current_trigger = (index == current_trigger_index)

		list_triggers[index].set_collision_mask_value(2, !is_current_trigger)
		list_triggers[index].set_collision_mask_value(1, is_current_trigger)

func _physics_process(delta):
	move_and_slide()
	
func _on_rock_head_trigger_on_parent_collision():
	pass
	change_direction()
	
func get_next_direction(next_trigger_pos):
	await get_tree().create_timer(1.5).timeout
	var next_position = (next_trigger_pos - position)
	next_position = next_position.normalized()
	
	var copy = next_position.abs()
	if copy.x > copy.y:
		next_position.y = 0
	else:
		next_position.x = 0
		
	velocity = next_position * speed

func change_direction():
	current_trigger_index = (current_trigger_index + 1) % list_triggers.size()
	enable_triggers()
	get_next_direction(list_triggers[current_trigger_index].position)


func _on_area_2d_body_entered(body):
	print(body)
	pass # Replace with function body.
