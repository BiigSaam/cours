extends CharacterBody2D


@export var SPEED = 300.0
@export var JUMP_VELOCITY = -400.0
@export var max_jump = 2
@export var max_health = 20
@export var is_facing_right = true

@export var data:PlayerData

var jump_count = 0
var direction
var animator

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

signal on_death

func _ready():
	data.current_health = data.max_health
	animator = $Animator

func _process(delta):
	animation_manager()
	update_facing_direction()

func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y += gravity * delta

	if is_on_floor():
		jump_count = 0

	# Handle Jump.
	if Input.is_action_just_pressed("ui_accept") and jump_count < max_jump:
		velocity.y = data.jump_height
		jump_count += 1

	direction = Input.get_axis("left", "right")
	if direction:
		velocity.x = direction * data.speed
	else:
		velocity.x = move_toward(velocity.x, 0, data.speed)
		
	move_and_slide()
	
func update_facing_direction():
	if (direction > 0 && !is_facing_right or direction < 0 && is_facing_right):
		is_facing_right = !is_facing_right
		scale.x *= -1
	
func animation_manager():
	if is_on_floor():
		if velocity.x != 0:
			animator.play("run")
		else:
			animator.play("idle")
	else:
		if velocity.y < 0 and jump_count == 1:
			animator.play("jump")
		elif jump_count > 1 and velocity.y < 0:
			animator.play("double_jump")
		else:
			animator.play("fall")

func hit(damage):
	data.current_health -= damage 
