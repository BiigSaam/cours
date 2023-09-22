extends RigidBody2D

@export var bullet_prefab:PackedScene
@export var shoot_rate:Timer
@export var projectile_position:Marker2D
@export var data:EnemyData

var health = 0
var animator = null

func _ready():
	health = data.max_health
	animator = $Animator
	shoot_rate = get_node("ShootRate")

func _on_shoot_area_entered(body):
	if body.is_in_group("player"):
		shoot_rate.start()

func _on_shoot_area_exited(body):
	shoot_rate.stop()

func _on_animator_animation_finished(anim_name):
	if anim_name != "idle":
		animator.play("idle")

func shoot():
	var bullet = bullet_prefab.instantiate() as RigidBody2D
#	bullet.position = projectile_position.position
	bullet.global_position = get_node("ProjectilePosition").global_position
	
	var bullet_velocity =  Vector2.LEFT * bullet.speed
	bullet.linear_velocity = Vector2(bullet_velocity.x, 0)

	get_tree().root.get_child(0).add_child(bullet)
#	self.add_child(bullet)

func _on_shoot_rate_timeout():
	animator.play("shoot")

func _on_hit_area_entered(body):
	if "velocity" in body:
		body.velocity = Vector2(body.velocity.x, data.bounce_force)
		animator.play("hit")
		health -= 1
		if health <= 0:
			die()

func die():
	set_collision_mask_value(2, false)
	set_collision_mask_value(1, false)
	$EnemyCollision.set_deferred("disabled", true)
	$HitArea.set_deferred("disabled", true)
	$ShootArea.set_deferred("disabled", true) 
	
	apply_force(Vector2.UP * 12000)
	rotation_degrees = 45


func _on_visible_on_screen_notifier_2d_screen_exited():
	if health <= 0:
		queue_free()
