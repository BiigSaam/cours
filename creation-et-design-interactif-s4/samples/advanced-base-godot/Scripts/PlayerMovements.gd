extends CharacterBody2D


@export var SPEED = 300.0
@export var JUMP_VELOCITY = -400.0
@export var max_jump = 2
var jump_count = 0

signal foo

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

func _process(delta):
	animation_manager()

func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y += gravity * delta

	if is_on_floor():
		jump_count = 0

	# Handle Jump.
	if Input.is_action_just_pressed("ui_accept") and jump_count < max_jump:
		velocity.y = JUMP_VELOCITY
		jump_count += 1


	var direction = Input.get_axis("left", "right")
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		
	move_and_slide()
	
func animation_manager():
	if is_on_floor():
		if velocity.x != 0:
			%Animator.play("run")
		else:
			%Animator.play("idle")
	else:
		if velocity.y < 0 and jump_count == 1:
			%Animator.play("jump")
		elif jump_count > 1 and velocity.y < 0:
			%Animator.play("double_jump")
		else:
			%Animator.play("fall")

func _on_mouse_entered():
	print("ffe")

