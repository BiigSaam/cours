extends RigidBody2D

@export var bullet_prefab:PackedScene
@export var shoot_rate:Timer
@export var projectile_position:Marker2D
@export var data:EnemyData

var health = 0

func _ready():
	health = data.max_health
	shoot_rate = get_node("ShootRate")

func _on_shoot_area_entered(body):
	if body.is_in_group("player"):
		shoot_rate.start()

func _on_shoot_area_exited(body):
	shoot_rate.stop()

func _on_animator_animation_finished(anim_name):
	if anim_name == "shoot":
		$Animator.play("idle")

func shoot():
	var bullet = bullet_prefab.instantiate() as RigidBody2D
#	bullet.position = projectile_position.position
	bullet.global_position = get_node("ProjectilePosition").global_position
	
	var bullet_velocity =  Vector2.LEFT * bullet.speed
	bullet.linear_velocity = Vector2(bullet_velocity.x, 0)

	get_tree().root.get_child(0).add_child(bullet)
#	self.add_child(bullet)

func _on_shoot_rate_timeout():
	$Animator.play("shoot")

func _on_body_entered(body):
	print(body)
	print(body.position.y)

func _on_hit_area_entered(body):
	if "velocity" in body:
		body.velocity = Vector2(body.velocity.x, data.bounce_force)
		health -= 1
		if health <= 0:
			die()

func die():
	queue_free()
	
