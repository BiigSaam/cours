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
var cast_left
var cast_right
var cast_top
var cast_bottom
var is_dead = false
var is_stucked = false
var last_checkpoint:Vector2

@onready var collision = $CollisionShape2D

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

signal on_death

func _ready():
	data.current_health = data.max_health
	animator = $Animator
	cast_left = $LeftCast
	cast_right = $RightCast
	cast_top = $TopCast
	cast_bottom = $BottomCast
	last_checkpoint = position

func _process(delta):
	animation_manager()
	update_facing_direction()
	
func _physics_process(delta):
	if is_dead:
		return
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

	if is_sides_crushing(cast_left, cast_right) or is_sides_crushing(cast_top, cast_bottom):
			hit(100000)

func is_sides_crushing(begin_cast:RayCast2D, end_cast:RayCast2D):
	if begin_cast.is_colliding() and end_cast.is_colliding():
		if( 
			("velocity" in begin_cast.get_collider() and begin_cast.get_collider().velocity.length() > 0) or
			("velocity" in end_cast.get_collider() and end_cast.get_collider().velocity.length() > 0) 
		):
			return true
	return false
	
func update_facing_direction():
	if (direction > 0 && !is_facing_right or direction < 0 && is_facing_right):
		is_facing_right = !is_facing_right
		scale.x *= -1
	
func animation_manager():
	if is_dead:
		if animator.animation != "die":
			animator.play_backwards("die")
	else:
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
	if(data.current_health <= 0):
		die()
	
func die():
	on_death.emit()
	is_dead = true
	collision.disabled = true
	print("die 2 !")
