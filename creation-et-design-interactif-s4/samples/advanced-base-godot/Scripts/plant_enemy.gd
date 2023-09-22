extends RigidBody2D

@export var bullet_prefab:PackedScene
@export var shoot_rate:Timer
@export var projectile_position:Marker2D

signal on_detect_area_enter(body)

func _on_area_2d_body_entered(body):
	print(body)
	if body.is_in_group("player"):
		shoot_rate.start()
		
		pass
#	$Animator.play("shoot")
#	print(_body)
#	shoot_rate.start()
#	var bullet = bullet_prefab.instantiate() as Area2D
#
#	add_child(bullet)
#	on_detect_area_enter.emit(body)

func _on_shoot_area_body_exited(body):
	shoot_rate.stop()

func _on_animator_animation_finished(anim_name):
	if anim_name == "shoot":
		$Animator.play("idle")

func shoot():
	var bullet = bullet_prefab.instantiate() as RigidBody2D
	bullet.position = projectile_position.position
	var bullet_velocity =  Vector2.LEFT * bullet.speed
	bullet.linear_velocity = Vector2(bullet_velocity.x, 0)
	add_child(bullet)

func _on_shoot_rate_timeout():
	$Animator.play("shoot")
