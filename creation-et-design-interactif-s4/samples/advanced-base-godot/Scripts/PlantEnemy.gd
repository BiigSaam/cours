extends RigidBody2D

@export var bullet_prefab:PackedScene
@export var shoot_rate:Timer

signal on_detect_area_enter(body)

func _on_area_2d_body_entered(body):
#	$Animator.play("shoot")
	print(body)
#	var bullet = bullet_prefab.instantiate() as Area2D
#
#	add_child(bullet)
#	on_detect_area_enter.emit(body)



func _on_animator_animation_finished():
	print("Heeedd")
#	$Animator.play("idle")

func shoot():
	pass
