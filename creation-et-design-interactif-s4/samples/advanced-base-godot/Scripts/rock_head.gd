extends CharacterBody2D

@export var speed:int = 750
@export var current_trigger_index = 0
@export var list_triggers:Array[RockHeadTrigger]
@export var hide = false

var last_animation_name = ""

var top_raycast
var bottom_raycast
var left_raycast
var right_raycast
var animator

var is_visible = false

var next_position = Vector2.ZERO

func _ready():
	if hide:
		queue_free()
		
	top_raycast = $TopRaycast
	bottom_raycast = $BottomRaycast
	left_raycast = $LeftRaycast
	right_raycast = $RightRaycast
	animator = $Animator
		
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
	
	if velocity.length() == 0:
		if next_position.y < -0.5 and top_raycast.is_colliding() and last_animation_name != "hit_top":
			animator.play("hit_top")
		elif next_position.y > 0.5 and bottom_raycast.is_colliding() and last_animation_name != "hit_bottom":
			animator.play("hit_bottom")
		elif next_position.x < -0.5 and left_raycast.is_colliding() and last_animation_name != "hit_left":
			animator.play("hit_left")
		elif next_position.x > 0.5 and right_raycast.is_colliding() and last_animation_name != "hit_right":
			animator.play("hit_right")
	else:
		animator.play("idle")
	
func _on_rock_head_trigger_on_parent_collision():
	change_direction()
	
func get_next_direction(next_trigger_pos):
	await get_tree().create_timer(1.5).timeout
	next_position = (next_trigger_pos - position)
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
	
func on_crush():
	if animator.animation != "idle":
		last_animation_name = animator.animation
		animator.play("idle")

func _on_animator_animation_finished():
	on_crush()

func _on_visible_on_screen_notifier_2d_screen_entered():
	is_visible = true

func _on_visible_on_screen_notifier_2d_screen_exited():
	is_visible = false
